using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;

namespace Game.Logging
{
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

	public sealed class LogRouterHandler : ILogHandler
	{
		private readonly ILogHandler fallback;
		private readonly LogRouterConfig cfg;

		private readonly Dictionary<string, (int c, float t)> dupe = new();
		private readonly HashSet<string> shownOnce = new(StringComparer.OrdinalIgnoreCase);
		private readonly float installedAt;

		public LogRouterHandler(ILogHandler fb, LogRouterConfig config)
		{
			fallback = fb;
			cfg = config;
			installedAt = Time.realtimeSinceStartup;
		}

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
				LogType.Log       => cfg.showLog,
				LogType.Warning   => cfg.showWarning,
				LogType.Error     => cfg.showError,
				LogType.Assert    => cfg.showAssert,
				LogType.Exception => cfg.showException,
				_ => true
			};
		}

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

		private static string ExtractCategory(string message)
		{
			if (!string.IsNullOrEmpty(message) && message[0] == '[')
			{
				int end = message.IndexOf(']');
				if (end > 0 && end < 40) return message.Substring(0, end + 1);
			}
			return "[Uncat]";
		}

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

	public static class LogRouterBootstrap
	{
		private static bool installed;

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
