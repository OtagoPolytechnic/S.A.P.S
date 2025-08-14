#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using Game.Logging;

/// Editor menu for creating/opening the config and common actions.
public static class LoggingMenu
{
	// Find existing config in project; create one in Assets/Resources if missing.
	private static LogRouterConfig GetOrCreateConfig()
	{
		string[] guids = AssetDatabase.FindAssets("t:LogRouterConfig");
		if (guids != null && guids.Length > 0)
		{
			string path = AssetDatabase.GUIDToAssetPath(guids[0]);
			LogRouterConfig found = AssetDatabase.LoadAssetAtPath<LogRouterConfig>(path);
			if (found != null) return found;
		}

		if (!AssetDatabase.IsValidFolder("Assets/Resources"))
			AssetDatabase.CreateFolder("Assets", "Resources");

		string assetPath = "Assets/Resources/LogRouterConfig.asset";
		LogRouterConfig cfg = AssetDatabase.LoadAssetAtPath<LogRouterConfig>(assetPath);
		if (cfg == null)
		{
			cfg = ScriptableObject.CreateInstance<LogRouterConfig>();
			AssetDatabase.CreateAsset(cfg, assetPath);
			AssetDatabase.SaveAssets();
		}
		return cfg;
	}

	// Save and focus an asset in the Project window.
	private static void SaveAndPing(Object obj)
	{
		EditorUtility.SetDirty(obj);
		AssetDatabase.SaveAssets();
		Selection.activeObject = obj;
		EditorGUIUtility.PingObject(obj);
	}

	// -------- Menu: Config --------

	[MenuItem("Tools/Logging/Create or Open Config")]
	public static void CreateOrOpenConfig()
	{
		LogRouterConfig cfg = GetOrCreateConfig();
		SaveAndPing(cfg);
	}

	// -------- Menu: Folders --------

	[MenuItem("Tools/Logging/Open Logs Folder")]
	public static void OpenLogsFolder()
	{
		LogRouterConfig cfg = GetOrCreateConfig();
		string subdir = string.IsNullOrEmpty(cfg.logDir) ? "Logs" : cfg.logDir;

		string root = Path.Combine(Application.persistentDataPath, subdir);
		if (!Directory.Exists(root)) Directory.CreateDirectory(root);

		EditorUtility.RevealInFinder(root);
	}

	// -------- Menu: Console Gate --------

	[MenuItem("Tools/Logging/Console ▸ Only Errors")]
	public static void ConsoleOnlyErrors()
	{
		LogRouterConfig cfg = GetOrCreateConfig();
		cfg.consoleMin = LogType.Error;
		SaveAndPing(cfg);
		Debug.Log("LogRouter: Console minimum set to Error (warnings hidden).");
	}

	[MenuItem("Tools/Logging/Console ▸ Allow Warnings")]
	public static void ConsoleAllowWarnings()
	{
		LogRouterConfig cfg = GetOrCreateConfig();
		cfg.consoleMin = LogType.Warning;
		SaveAndPing(cfg);
		Debug.Log("LogRouter: Console minimum set to Warning.");
	}

	// -------- Menu: Suppression Preset --------

	[MenuItem("Tools/Logging/Add Default Suppress Rules")]
	public static void AddDefaultSuppress()
	{
		LogRouterConfig cfg = GetOrCreateConfig();

		string[] defaults = new string[]
		{
			"Can not play a disabled audio source",
			"PlayOneShot was called with a null AudioClip",
			"A collider used by an Interactable object is already registered"
		};

		List<string> list = new List<string>();
		if (cfg.suppressIfContains != null && cfg.suppressIfContains.Length > 0)
			list.AddRange(cfg.suppressIfContains);

		bool changed = false;
		for (int i = 0; i < defaults.Length; i++)
		{
			string rule = defaults[i];
			bool present = false;

			for (int j = 0; j < list.Count; j++)
			{
				if (string.Equals(list[j], rule, System.StringComparison.OrdinalIgnoreCase))
				{
					present = true;
					break;
				}
			}

			if (!present)
			{
				list.Add(rule);
				changed = true;
			}
		}

		if (changed)
		{
			cfg.suppressIfContains = list.ToArray();
			SaveAndPing(cfg);
			Debug.Log("LogRouter: Default suppress rules added/merged.");
		}
		else
		{
			Debug.Log("LogRouter: All default suppress rules already present.");
		}
	}

	// -------- Menu: Utilities --------

	[MenuItem("Tools/Logging/Show Persistent Path")]
	public static void ShowPersistentPath()
	{
		Debug.Log("persistentDataPath: " + Application.persistentDataPath);
		EditorGUIUtility.systemCopyBuffer = Application.persistentDataPath;
	}
}
#endif
