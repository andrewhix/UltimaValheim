using BepInEx;
using BepInEx.Configuration;

namespace UltimaValheim.Core
{
    /// <summary>
    /// Handles configuration bindings and shared config values.
    /// </summary>
    public class ConfigManager
    {
        public ConfigEntry<bool> DebugMode { get; private set; }

        internal void Bind(BaseUnityPlugin plugin)
        {
            DebugMode = plugin.Config.Bind("Core", "DebugMode", false, "Enable extra debug output");
        }
    }
}
