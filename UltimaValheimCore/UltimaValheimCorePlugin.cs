using BepInEx;
using HarmonyLib;

namespace UltimaValheim.Core
{
    [BepInPlugin(ModGuid, ModName, ModVersion)]
    [BepInDependency(Jotunn.Main.ModGuid)]
    public class UltimaValheimCorePlugin : BaseUnityPlugin
    {
        public const string ModGuid = "com.valheim.ultima.core";
        public const string ModName = "Ultima Valheim Core";
        public const string ModVersion = "0.1.0";

        private Harmony _harmony;

        private void Awake()
        {
            _harmony = new Harmony(ModGuid);
            _harmony.PatchAll();

            CoreAPI.Initialize(Logger);
            CoreAPI.Config.Bind(this);

            CoreAPI.Log.LogInfo($"{ModName} {ModVersion} initialized!");

        }

        private void OnDestroy()
        {
            _harmony?.UnpatchSelf();
        }
    }
}
