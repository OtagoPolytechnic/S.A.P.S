using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;

namespace Game.Logging
{

	/// <summary>
    /// Config for the Log Router: what to show in the Console, substring-based suppression,
    /// rate-limits, optional file mirroring, and a short startup mute. Stored in Resources as
    /// "LogRouterConfig" and read on boot by <see cref="LogRouterBootstrap"/>.
    /// </summary>
	[CreateAssetMenu(fileName = "LogRouterConfig", menuName = "Logging/Log Router Config")]
	public class LogRouterConfig : ScriptableObject
	{
		[Header("Console Types (tick to show in Console)")]
		public bool showLog = true;
		public bool showWarning = true;
		public bool showError = true;
		public bool showAssert = true;
		public bool showException = true;

		[Header("Suppress by Content (substring match, case-insensitive)")]
		public string[] suppressIfContains = {
			"A collider used by an Interactable object is already registered"
		};

		[Header("Show Once If Contains")]
		public string[] showOnceIfContains = Array.Empty<string>();

		[Header("Duplicate Rate-Limit")]
		public int maxPerWindow = 5; // 0 = off
		public float windowSeconds = 10f;

		[Header("Optional File Logging")]
		public bool writeCategoryFiles = false;
		public bool mirrorSuppressedToFile = false;
		public string logDir = "Logs";

		[Header("Startup Mute")]
		[Tooltip("Suppress non-error logs for this many seconds after Play starts.")]
		public float muteOnStartSeconds = 0f;

		[Header("Debug")]
		public bool printInstallLine = true;

		// Legacy field kept only so old assets don’t break in inspector (unused in code)
		#pragma warning disable 414
		[SerializeField, HideInInspector] private LogType consoleMin_Legacy = LogType.Warning;
		#pragma warning restore 414
	}
	
    /// <summary>
	/// An ILogHandler that routes Unity logs through suppression, "show once", rate-limits,
	/// and optional file mirroring — then forwards allowed logs to Unity's default handler.
	/// </summary>
	public sealed class LogRouterHandler : ILogHandler
	{
		private readonly ILogHandler fallback;
		private readonly LogRouterConfig cfg;

        /// <summary>Duplicate detector: key = "type|text", value = (count, first-timestamp).</summary>
		private readonly Dictionary<string, (int c, float t)> dupe = new();

		 /// <summary>Tracks which "show once" substrings have already been seen.</summary>
		private readonly HashSet<string> shownOnce = new(StringComparer.OrdinalIgnoreCase);
		private readonly float installedAt;

        /// <summary>
        /// Creates a router around Unity's default handler.
        /// </summary>
        /// <param name="fb">Fallback (usually Unity's default log handler).</param>
        /// <param name="config">Active router configuration.</param>
		public LogRouterHandler(ILogHandler fb, LogRouterConfig config)
		{
			fallback = fb;
			cfg = config;
			installedAt = Time.realtimeSinceStartup;
		}

        /// <summary>
        /// Handles exceptions according to visibility settings and optional file mirroring.
        /// </summary>
		public void LogException(Exception exception, UnityEngine.Object context)
		{
			if (!cfg.showException)
			{
				if (cfg.mirrorSuppressedToFile) WriteToFile("[SuppressedType]", exception.ToString());
				return;
			}
			fallback.LogException(exception, context);
			if (cfg.writeCategoryFiles) WriteToFile("[Exception]", exception.ToString());
		}

        /// <summary>
        /// Main log path: applies type visibility, startup mute, content suppression,
        /// "show once", rate-limit, optional file write, then forwards to fallback.
        /// </summary>
        /// <param name="type">Unity log type (Log/Warning/Error/Assert/Exception).</param>
        /// <param name="context">Unity context object (optional).</param>
        /// <param name="format">Message format string.</param>
        /// <param name="args">Format args.</param>
		public void LogFormat(LogType type, UnityEngine.Object context, string format, params object[] args)
		{
			string msg = SafeFormat(format, args);


			if (!IsTypeEnabled(type))
			{
				if (cfg.mirrorSuppressedToFile) WriteToFile("[SuppressedType]", $"[{type}] {msg}");
				return;
			}

			// Startup mute for non-critical
			if (cfg.muteOnStartSeconds > 0f
				&& (Time.realtimeSinceStartup - installedAt) < cfg.muteOnStartSeconds
				&& type != LogType.Error && type != LogType.Assert && type != LogType.Exception)
			{
				if (cfg.mirrorSuppressedToFile) WriteToFile("[SuppressedStart]", $"[{type}] {msg}");
				return;
			}

			string text = StripTimestamp(msg);


			if (ContainsAny(text, cfg.suppressIfContains))
			{
				if (cfg.mirrorSuppressedToFile) WriteToFile("[Suppressed]", $"[{type}] {text}");
				return;
			}


			string onceKey = FirstHit(text, cfg.showOnceIfContains);
			if (onceKey != null && shownOnce.Contains(onceKey))
			{
				if (cfg.mirrorSuppressedToFile) WriteToFile("[SuppressedOnce]", $"[{type}] {text}");
				return;
			}
			if (onceKey != null) shownOnce.Add(onceKey);


			if (cfg.maxPerWindow > 0 && IsRateLimited(type, text))
			{
				if (cfg.mirrorSuppressedToFile) WriteToFile("[SuppressedRate]", $"[{type}] {text}");
				return;
			}


			if (cfg.writeCategoryFiles) WriteToFile(ExtractCategory(text), $"[{type}] {text}");

			fallback.LogFormat(type, context, format, args);
		}

		private bool IsTypeEnabled(LogType type)
		{
			return type switch
			{
				LogType.Log => cfg.showLog,
				LogType.Warning => cfg.showWarning,
				LogType.Error => cfg.showError,
				LogType.Assert => cfg.showAssert,
				LogType.Exception => cfg.showException,
				_ => true
			};
		}

        /// <summary>Returns the first matching substring from <paramref name="needles"/>, or null.</summary>
		private static string SafeFormat(string format, object[] args)
		{
			try { return string.Format(format, args); } catch { return format; }
		}

		private static bool ContainsAny(string message, string[] needles)
		{
			if (string.IsNullOrEmpty(message) || needles == null) return false;
			for (int i = 0; i < needles.Length; i++)
			{
				if (!string.IsNullOrEmpty(needles[i]) &&
					message.IndexOf(needles[i], StringComparison.OrdinalIgnoreCase) >= 0)
					return true;
			}
			return false;
		}

		private static string FirstHit(string message, string[] needles)
		{
			if (string.IsNullOrEmpty(message) || needles == null) return null;
			for (int i = 0; i < needles.Length; i++)
			{
				if (!string.IsNullOrEmpty(needles[i]) &&
					message.IndexOf(needles[i], StringComparison.OrdinalIgnoreCase) >= 0)
					return needles[i];
			}
			return null;
		}

        /// <summary>
        /// Sliding-window duplicate limiter. Counts identical "type|text" messages within
        /// <see cref="LogRouterConfig.windowSeconds"/> and suppresses when above <see cref="LogRouterConfig.maxPerWindow"/>.
        /// </summary>
		private bool IsRateLimited(LogType type, string text)
		{
			string key = type + "|" + text;
			float now = Time.realtimeSinceStartup;

			if (!dupe.TryGetValue(key, out var e))
			{
				dupe[key] = (1, now);
				return false;
			}

			if (now - e.t > cfg.windowSeconds)
			{
				dupe[key] = (1, now);
				return false;
			}

			e.c++;
			dupe[key] = e;
			return e.c > cfg.maxPerWindow;
		}

        /// <summary>
        /// Writes a single line to a category file under <c>Application.persistentDataPath/logDir</c>.
        /// Swallows IO errors by design.
        /// </summary>
		private void WriteToFile(string category, string line)
		{
			try
			{
				string root = Path.Combine(Application.persistentDataPath, cfg.logDir);
				if (!Directory.Exists(root)) Directory.CreateDirectory(root);
				string name = string.IsNullOrEmpty(category) ? "Uncat" : category.Trim('[', ']');
				File.AppendAllText(Path.Combine(root, name + ".log"),
					DateTime.UtcNow.ToString("O") + " " + line + "\n");
			}
			catch { }
		}

        /// <summary>
        /// Extracts a leading bracketed token to use as a category (e.g. "[AI] Something…").
        /// Returns "[Uncat]" if none found.
        /// </summary>
		private static string ExtractCategory(string message)
		{
			if (!string.IsNullOrEmpty(message) && message[0] == '[')
			{
				int end = message.IndexOf(']');
				if (end > 0 && end < 40) return message.Substring(0, end + 1);
			}
			return "[Uncat]";
		}

        /// <summary>
        /// Strips a short leading bracketed timestamp (e.g. "[12:34:56] …"); returns original otherwise.
        /// </summary>
		private static string StripTimestamp(string msg)
		{
			if (string.IsNullOrEmpty(msg) || msg[0] != '[') return msg;
			int r = msg.IndexOf(']');
			if (r <= 0 || r + 1 >= msg.Length) return msg;
			string inside = msg.Substring(1, r - 1);
			bool looksTime = inside.IndexOf(':') >= 0 && inside.Length <= 8;
			if (!looksTime) return msg;
			int i = r + 1;
			while (i < msg.Length && char.IsWhiteSpace(msg[i])) i++;
			return msg.Substring(i);
		}
	}

    /// <summary>
    /// Bootstraps the Log Router at startup. Loads a <see cref="LogRouterConfig"/> from Resources (or a
    /// temporary default), installs <see cref="LogRouterHandler"/>, and sets Unity's logger to pass all types.
    /// </summary>
	public static class LogRouterBootstrap
	{
		private static bool installed;

        /// <summary>
        /// Installs the router once with the provided config and switches Unity's logger to route through it.
        /// </summary>
        /// <param name="cfg">Configuration to apply to the router.</param>
		public static void InstallWithConfig(LogRouterConfig cfg)
		{
			if (installed) return;
			var def = Debug.unityLogger.logHandler;

			// Capture everything; we filter in our handler so Unity never “defaults to Warning”
			Debug.unityLogger.filterLogType = LogType.Log;
			Debug.unityLogger.logHandler = new LogRouterHandler(def, cfg);

			installed = true;
			if (cfg.printInstallLine) Debug.Log("[LogRouter] Installed");
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Install()
		{
			var cfg = Resources.Load<LogRouterConfig>("LogRouterConfig")
					  ?? ScriptableObject.CreateInstance<LogRouterConfig>();
			InstallWithConfig(cfg);
		}
	}
}
