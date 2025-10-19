namespace UltimaValheim.Core
{
    /// <summary>
    /// Implemented by events that can be sent across the network.
    /// </summary>
    public interface INetworkEvent
    {
        /// <summary>Serialize this event into a ZPackage.</summary>
        void Serialize(ZPackage pkg);

        /// <summary>Deserialize this event from a ZPackage.</summary>
        void Deserialize(ZPackage pkg);
    }
}
