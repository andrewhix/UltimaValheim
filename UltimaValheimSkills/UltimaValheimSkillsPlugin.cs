using BepInEx;
using BepInEx.Logging;
using UltimaValheim.Core;
using UltimaValheim.Skills;

namespace UltimaValheim.Skills
{
    [BepInPlugin(ModGUID, ModName, ModVersion)]
    [BepInDependency("com.valheim.ultima.core", BepInDependency.DependencyFlags.HardDependency)]
    public class UltimaValheimSkillsPlugin : BaseUnityPlugin, ICoreModule
    {
        public const string ModGUID = "com.valheim.ultima.skills";
        public const string ModName = "Ultima Valheim Skills";
        public const string ModVersion = "1.0.0";

        internal static ManualLogSource Log;

        private void Awake()
        {
            Log = Logger;
            Log.LogInfo("[UVC Skills] Initializing...");

            // Register this module with Core
            CoreAPI.RegisterModule(this);

            // Initialize the Skills system (ServerCharacters optional)
            PlayerSkillManager.Initialize();

            Log.LogInfo("[UVC Skills] Initialization complete.");
        }

        // ICoreModule
        public void OnCoreReady()
        {
            Log.LogInfo("[UVC Skills] Core is ready. Skills module synchronized.");
        }
    }
}
