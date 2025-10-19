using System;
using System.Reflection;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using UnityEngine;

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

            // Initialize optional ServerCharacters bridge
            Players.Initialize();

            // Notify all registered modules that Core is ready
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

        // -------------------------------------------------------------
        //  Player Integration (ServerCharacters Bridge)
        // -------------------------------------------------------------
        public static class Players
        {
            public static event Action<Player> OnLoaded;
            public static event Action<Player> OnSaving;
            public static event Action<Player> OnDisconnected;

            private static bool _initialized;
            private static Type _apiType;
            private static EventInfo _evtLoaded, _evtSaving, _evtDisconnected;

            public static void Initialize()
            {
                if (_initialized)
                    return;

                try
                {
                    // Look for the ServerCharacters plugin (Thunderstore ID: blaxxun-boop.servercharacters)
                    if (Chainloader.PluginInfos.TryGetValue("blaxxun-boop.servercharacters", out var pluginInfo))
                    {
                        CoreAPI.Log.LogInfo("[UVC CoreAPI] Detected ServerCharacters plugin — attempting to bind events.");

                        // Get the API type via reflection
                        _apiType = pluginInfo.Instance.GetType().Assembly.GetType("ServerCharacters.API");
                        if (_apiType == null)
                        {
                            CoreAPI.Log.LogWarning("[UVC CoreAPI] Could not locate ServerCharacters.API type.");
                            return;
                        }

                        _evtLoaded = _apiType.GetEvent("OnPlayerLoaded");
                        _evtSaving = _apiType.GetEvent("OnPlayerSaving");
                        _evtDisconnected = _apiType.GetEvent("OnPlayerDisconnected");

                        if (_evtLoaded != null)
                            _evtLoaded.AddEventHandler(null, (Action<Player>)((p) => OnLoaded?.Invoke(p)));
                        if (_evtSaving != null)
                            _evtSaving.AddEventHandler(null, (Action<Player>)((p) => OnSaving?.Invoke(p)));
                        if (_evtDisconnected != null)
                            _evtDisconnected.AddEventHandler(null, (Action<Player>)((p) => OnDisconnected?.Invoke(p)));

                        CoreAPI.Log.LogInfo("[UVC CoreAPI] Successfully bound to ServerCharacters events.");
                        _initialized = true;
                    }
                    else
                    {
                        CoreAPI.Log.LogWarning("[UVC CoreAPI] ServerCharacters not detected. Player persistence disabled.");
                    }
                }
                catch (Exception ex)
                {
                    CoreAPI.Log.LogWarning($"[UVC CoreAPI] Failed to bind ServerCharacters events: {ex}");
                }
            }
        }
    }
}
