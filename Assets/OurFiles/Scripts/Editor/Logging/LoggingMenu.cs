#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;
using Game.Logging;

namespace Game.Logging.Editor
{
	public static class LoggingMenu
	{
		private static LogRouterConfig GetOrCreateConfig()
		{
			string[] guids = AssetDatabase.FindAssets("t:LogRouterConfig");
			if (guids != null && guids.Length > 0)
			{
				string path = AssetDatabase.GUIDToAssetPath(guids[0]);
				var found = AssetDatabase.LoadAssetAtPath<LogRouterConfig>(path);
				if (found != null) return found;
			}

			if (!AssetDatabase.IsValidFolder("Assets/Resources"))
				AssetDatabase.CreateFolder("Assets", "Resources");

			string assetPath = "Assets/Resources/LogRouterConfig.asset";
			var cfg = AssetDatabase.LoadAssetAtPath<LogRouterConfig>(assetPath);
			if (cfg == null)
			{
				cfg = ScriptableObject.CreateInstance<LogRouterConfig>();
				AssetDatabase.CreateAsset(cfg, assetPath);
				AssetDatabase.SaveAssets();
			}
			return cfg;
		}

		private static void Save(Object obj)
		{
			EditorUtility.SetDirty(obj);
			AssetDatabase.SaveAssets();
		}

		[MenuItem("Tools/Logging/Create or Open Config")]
		public static LogRouterConfig CreateOrOpenConfig()
		{
			var cfg = GetOrCreateConfig();
			Selection.activeObject = cfg;
			EditorGUIUtility.PingObject(cfg);
			LogRouterConfigWindow.Show(cfg);
			return cfg;
		}

		[MenuItem("Tools/Logging/Open Logs Folder")]
		public static void OpenLogsFolder()
		{
			var cfg = GetOrCreateConfig();
			string subdir = string.IsNullOrEmpty(cfg.logDir) ? "Logs" : cfg.logDir;
			string root = Path.Combine(Application.persistentDataPath, subdir);
			if (!Directory.Exists(root)) Directory.CreateDirectory(root);
			EditorUtility.RevealInFinder(root);
		}
	}
}
#endif
