#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using Game.Logging;

namespace Game.Logging.Editor
{
	public class LogRouterConfigWindow : EditorWindow
	{
		private LogRouterConfig cfg;
		private Vector2 scroll;
		private string newRule = "";

		private static readonly string[] commonSpam = new[]
		{
			"A collider used by an Interactable object is already registered",
		};

		// master set of suppress rules 
		private readonly HashSet<string> scratch = new HashSet<string>(System.StringComparer.OrdinalIgnoreCase);

		public static void Show(LogRouterConfig target)
		{
			var win = GetWindow<LogRouterConfigWindow>("Log Router");
			win.cfg = target;
			win.minSize = new Vector2(420, 420);
			win.RefreshScratch();
			win.Show();
		}

		private void OnEnable()
		{
			if (cfg == null)
			{
				string[] guids = AssetDatabase.FindAssets("t:LogRouterConfig");
				if (guids.Length > 0)
				{
					string path = AssetDatabase.GUIDToAssetPath(guids[0]);
					cfg = AssetDatabase.LoadAssetAtPath<LogRouterConfig>(path);
				}
			}
			RefreshScratch();
		}

		private void RefreshScratch()
		{
			scratch.Clear();
			if (cfg != null && cfg.suppressIfContains != null)
			{
				foreach (var s in cfg.suppressIfContains)
					if (!string.IsNullOrWhiteSpace(s)) scratch.Add(s.Trim());
			}
		}

		private void SaveScratch()
		{
			if (cfg == null) return;
			cfg.suppressIfContains = new List<string>(scratch).ToArray();
			EditorUtility.SetDirty(cfg);
			AssetDatabase.SaveAssets();
		}

		private GUIStyle Header => new GUIStyle(EditorStyles.boldLabel){ fontSize = 12 };

		private void OnGUI()
		{
			if (cfg == null)
			{
				EditorGUILayout.HelpBox("No LogRouterConfig found. Use Tools ▸ Logging ▸ Open Config.", MessageType.Info);
				if (GUILayout.Button("Open Config")) LoggingMenu.CreateOrOpenConfig(); // update if you renamed method
				return;
			}

			scroll = EditorGUILayout.BeginScrollView(scroll);

			// ==== Console toggles ====
			EditorGUILayout.LabelField("Console Types", Header);
			cfg.showLog       = EditorGUILayout.ToggleLeft("Logs", cfg.showLog);
			cfg.showWarning   = EditorGUILayout.ToggleLeft("Warnings", cfg.showWarning);
			cfg.showError     = EditorGUILayout.ToggleLeft("Errors", cfg.showError);
			cfg.showAssert    = EditorGUILayout.ToggleLeft("Asserts", cfg.showAssert);
			cfg.showException = EditorGUILayout.ToggleLeft("Exceptions", cfg.showException);

			using (new EditorGUILayout.HorizontalScope())
			{
				if (GUILayout.Button("Only Errors")) {
					cfg.showLog = false; cfg.showWarning = false; cfg.showError = true; cfg.showAssert = true; cfg.showException = true;
				}
				if (GUILayout.Button("Errors + Warnings")) {
					cfg.showLog = false; cfg.showWarning = true; cfg.showError = true; cfg.showAssert = true; cfg.showException = true;
				}
				if (GUILayout.Button("Everything")) {
					cfg.showLog = cfg.showWarning = cfg.showError = cfg.showAssert = cfg.showException = true;
				}
			}

			// ==== Startup mute ====
			EditorGUILayout.Space(8);
			EditorGUILayout.LabelField("Startup Mute", Header);
			cfg.muteOnStartSeconds = EditorGUILayout.Slider(new GUIContent("Mute seconds after Play"), cfg.muteOnStartSeconds, 0f, 10f);

			// ==== Rate limit ====
			EditorGUILayout.Space(8);
			EditorGUILayout.LabelField("Rate Limit", Header);
			cfg.maxPerWindow = EditorGUILayout.IntSlider(new GUIContent("Max per window (0 = off)"), cfg.maxPerWindow, 0, 50);
			cfg.windowSeconds = EditorGUILayout.Slider(new GUIContent("Window (sec)"), cfg.windowSeconds, 1f, 60f);

			// ==== File logging ====
			EditorGUILayout.Space(8);
			EditorGUILayout.LabelField("File Logging (optional)", Header);
			cfg.writeCategoryFiles = EditorGUILayout.Toggle("Write category files", cfg.writeCategoryFiles);
			cfg.mirrorSuppressedToFile = EditorGUILayout.Toggle("Mirror suppressed", cfg.mirrorSuppressedToFile);
			cfg.logDir = EditorGUILayout.TextField("Log subfolder", string.IsNullOrEmpty(cfg.logDir) ? "Logs" : cfg.logDir);

			// ==== Unified suppress rules ====
			EditorGUILayout.Space(10);
			EditorGUILayout.LabelField("Suppress by Content", Header);

			using (new EditorGUILayout.HorizontalScope())
			{
				if (GUILayout.Button("Select All", GUILayout.Width(100)))
				{
					foreach (var s in commonSpam) scratch.Add(s);
				}
				if (GUILayout.Button("Clear All", GUILayout.Width(100)))
				{
					foreach (var s in commonSpam) scratch.Remove(s);
				}
			}

			EditorGUILayout.Space(2);

			List<string> allRules = new List<string>(scratch);
			foreach (var s in commonSpam)
				if (!allRules.Contains(s)) allRules.Insert(0, s);

			foreach (var s in allRules)
			{
				bool on = scratch.Contains(s);
				bool next = EditorGUILayout.ToggleLeft(s, on);
				if (next && !on) scratch.Add(s);
				else if (!next && on) scratch.Remove(s);
			}

			// Add new rule field
			using (new EditorGUILayout.HorizontalScope())
			{
				newRule = EditorGUILayout.TextField(new GUIContent("Add rule (substring)"), newRule);
				if (GUILayout.Button("Add", GUILayout.Width(60)))
				{
					if (!string.IsNullOrWhiteSpace(newRule))
					{
						scratch.Add(newRule.Trim());
						newRule = "";
						SaveScratch();
					}
				}
			}

			// ==== Save/Logs buttons ====
			EditorGUILayout.Space(10);
			using (new EditorGUILayout.HorizontalScope())
			{
				if (GUILayout.Button("Save"))
				{
					SaveScratch();
					EditorUtility.SetDirty(cfg);
					AssetDatabase.SaveAssets();
					ShowNotification(new GUIContent("Saved"));
				}
				if (GUILayout.Button("Open Logs Folder"))
				{
					LoggingMenu.OpenLogsFolder();
				}
			}

			EditorGUILayout.EndScrollView();
		}
	}
}
#endif
