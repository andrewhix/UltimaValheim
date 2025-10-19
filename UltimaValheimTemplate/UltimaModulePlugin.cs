using BepInEx;
using UltimaValheim.Core;

namespace UltimaValheim.Template
{
    [BepInPlugin(ModGuid, ModName, ModVersion)]
    [BepInDependency(""com.valheim.ultima.core"")]
    public class UltimaModulePlugin : BaseUnityPlugin
    {
        public const string ModGuid = ""com.valheim.ultima.template"";
        public const string ModName = ""Ultima Valheim Module Template"";
        public const string ModVersion = ""0.1.0"";

        private ModuleBase _module;

        private void Awake()
        {
            CoreAPI.Log.LogInfo($""[{ModName}] Initializing..."");
            _module = new ModuleBase(ModName);
            CoreAPI.RegisterModule(_module);
            CoreAPI.Log.LogInfo($""[{ModName}] Registered successfully with Core."");
        }
    }
}
