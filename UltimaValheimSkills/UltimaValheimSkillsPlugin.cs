using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UltimaValheim.Core;

namespace UltimaValheim.Skills
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency("com.valheim.ultima.core")]
    public class UltimaValheimSkillsPlugin : BaseUnityPlugin, ICoreModule
    {
        public const string PluginGUID = "com.valheim.ultima.skills";
        public const string PluginName = "Ultima Valheim Skills";
        public const string PluginVersion = "0.2.0";

        internal static ManualLogSource Log;
        private Harmony _harmony;

        private void Awake()
        {
            Log = Logger;
            _harmony = new Harmony(PluginGUID);
            _harmony.PatchAll();

            CoreAPI.RegisterModule(this);
            Log.LogInfo("[UVC Skills] Loaded successfully and Harmony patches applied.");
        }

        public void OnCoreReady()
        {
            PlayerSkillManager.Initialize();

            // Register default skill definitions directly
            SkillRegistry.RegisterSkill(new SkillDef("Mining", "Improves ore yield and mining speed.", 100f, 1.2f));
            SkillRegistry.RegisterSkill(new SkillDef("Woodcutting", "Increases chopping efficiency.", 100f, 1.2f));
            SkillRegistry.RegisterSkill(new SkillDef("Running", "Improves movement stamina efficiency.", 100f, 1.2f));
            SkillRegistry.RegisterSkill(new SkillDef("Blocking", "Increases block effectiveness.", 100f, 1.2f));


            Log.LogInfo("[UVC Skills] Core ready — default skills registered.");
        }

        public void OnCoreShutdown()
        {
            PlayerSkillManager.Shutdown();
            _harmony?.UnpatchSelf();
            Log.LogInfo("[UVC Skills] Core shutting down — skills saved and patches removed.");
        }
    }
}
