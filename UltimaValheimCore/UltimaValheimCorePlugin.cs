using BepInEx;
using BepInEx.Logging;

namespace UltimaValheim.Core
{
    [BepInPlugin("com.valheim.ultima.core", "Ultima Valheim Core", "1.0.0")]
    public class UltimaValheimCorePlugin : BaseUnityPlugin
    {
        private void Awake()
        {
            CoreAPI.Initialize(Logger);
        }
    }
}
