using UltimaValheim.Core;

namespace UltimaValheim.Template
{
    public class ModuleBase : ICoreModule
    {
        private readonly string _moduleName;

        public ModuleBase(string moduleName)
        {
            _moduleName = moduleName;
        }

        public void OnCoreReady()
        {
            CoreAPI.Log.LogInfo($""[{_moduleName}] Core ready â€” module initialization complete."");
        }
    }
}
