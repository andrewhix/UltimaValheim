namespace UltimaValheim.Core
{
    /// <summary>
    /// Interface for modules that hook into the Core lifecycle.
    /// </summary>
    public interface ICoreModule
    {
        /// <summary>
        /// Called when Ultima Valheim Core finishes initializing.
        /// </summary>
        void OnCoreReady();
    }
}
