using System;
using System.Collections.Generic;

namespace UltimaValheim.Core
{
    /// <summary>
    /// Manages Core initialization and module registration lifecycle.
    /// </summary>
    public static class CoreLifecycle
    {
        private static readonly List<ICoreModule> _modules = new();
        private static bool _coreReady;

        /// <summary>
        /// Fired when the Core systems are initialized.
        /// </summary>
        public static event Action OnCoreReady;

        /// <summary>
        /// Called internally by CoreAPI once systems are initialized.
        /// </summary>
        internal static void NotifyCoreReady()
        {
            if (_coreReady) return;
            _coreReady = true;

            CoreAPI.Log.LogInfo("[UVC] Core lifecycle: CoreReady");

            // Fire global event for subscribers
            OnCoreReady?.Invoke();

            // Initialize any modules already registered
            foreach (var module in _modules)
            {
                try
                {
                    CoreAPI.Log.LogInfo($"[UVC] Initializing module: {module.GetType().Name}");
                    module.OnCoreReady();
                }
                catch (Exception ex)
                {
                    CoreAPI.Log.LogError($"[UVC] Error initializing module {module.GetType().Name}: {ex}");
                }
            }
        }

        /// <summary>
        /// Registers a module that should be initialized when the Core is ready.
        /// </summary>
        public static void RegisterModule(ICoreModule module)
        {
            if (module == null) return;

            _modules.Add(module);

            if (_coreReady)
            {
                // Core already ready, initialize immediately
                try
                {
                    CoreAPI.Log.LogInfo($"[UVC] Late module registration: {module.GetType().Name}");
                    module.OnCoreReady();
                }
                catch (Exception ex)
                {
                    CoreAPI.Log.LogError($"[UVC] Error initializing module {module.GetType().Name}: {ex}");
                }
            }
        }
    }
}
