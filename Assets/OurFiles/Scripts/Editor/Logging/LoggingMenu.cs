#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;
using Game.Logging;

namespace Game.Logging.Editor
{
	/// <summary>
    /// Editor menu items for configuring and accessing the log router.
    /// </summary>
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
		
        /// <summary>
		/// Opens or creates the config, selects it in the editor, and shows the config window.
		/// </summary>
		[MenuItem("Tools/Logging/Open Config")]
		public static LogRouterConfig CreateOrOpenConfig()
		{
			var cfg = GetOrCreateConfig();
			Selection.activeObject = cfg;
			EditorGUIUtility.PingObject(cfg);
			LogRouterConfigWindow.Show(cfg);
			return cfg;
		}

        /// <summary>
        /// Opens the configured logs folder in Finder/Explorer,
        /// creating it if missing.
        /// </summary>
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
