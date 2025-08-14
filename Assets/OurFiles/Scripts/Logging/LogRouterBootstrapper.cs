using UnityEngine;

namespace Game.Logging
{
    [DefaultExecutionOrder(-32000)]
    public sealed class LogRouterBootstrapper : MonoBehaviour
    {
        private static bool installedInScene;

        private void Awake()
        {
            if (installedInScene) return;
            installedInScene = true;

            LogRouterConfig cfg = Resources.Load<LogRouterConfig>("LogRouterConfig");
            if (cfg == null)
            {
                cfg = ScriptableObject.CreateInstance<LogRouterConfig>();
                cfg.consoleMin = LogType.Warning;
                cfg.printInstallLine = true;
            }

            LogRouterBootstrap.InstallWithConfig(cfg);
        }
    }
}
