using BepInEx.Logging;

namespace UltimaValheim.Core
{
    /// <summary>
    /// Provides centralized access to all core subsystems:
    /// Events, Persistence, Networking, Config, etc.
    /// </summary>
    public static class CoreAPI
    {
        public static ManualLogSource Log { get; private set; }
        public static EventBus Events { get; private set; }
        public static CoreEventRouter Router { get; private set; }
        public static NetworkManager Network { get; private set; }
        public static PersistenceManager Persistence { get; private set; }
        public static ConfigManager Config { get; private set; }

        internal static void Initialize(ManualLogSource log)
        {
            Log = log;
            Events = new EventBus();
            Router = new CoreEventRouter();
            Network = new NetworkManager();
            Persistence = new PersistenceManager();
            Config = new ConfigManager();

            Log.LogInfo("[UVC CoreAPI] Initialized all core systems.");

            // Notify all registered modules that Core is ready.
            CoreLifecycle.NotifyCoreReady();
        }

        public static void RegisterModule(ICoreModule module)
        {
            CoreLifecycle.RegisterModule(module);
        }

        // -------------------------------------------------------------
        //  Skills Sub-API (used by modules like Combat, Magic, Mining)
        // -------------------------------------------------------------
        public static class Skills
        {
            public static void RegisterSkill(string name, string description = "")
            {
                if (Events == null)
                {
                    Log.LogWarning("[UVC CoreAPI] Tried to register skill before Events system initialized.");
                    return;
                }

                Log.LogInfo($"[UVC CoreAPI] Registering skill '{name}' via Core event.");
                Events.Publish("RegisterSkill", new SkillRegistrationData(name, description));
            }
        }

        public record SkillRegistrationData(string Name, string Description);
    }
}
