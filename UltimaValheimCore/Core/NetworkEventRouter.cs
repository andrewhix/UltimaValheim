using System;
using System.Collections.Generic;
using UnityEngine;

namespace UltimaValheim.Core
{
    /// <summary>
    /// Handles type-safe network event synchronization using ZRoutedRpc.
    /// </summary>
    public class NetworkEventRouter
    {
        private readonly Dictionary<Type, Action<INetworkEvent>> _handlers = new();
        private readonly string _rpcBase = "UVC_NetEvent_";

        public NetworkEventRouter()
        {
            if (ZRoutedRpc.instance != null)
                CoreAPI.Log.LogInfo("[NetworkRouter] Initialized.");
            else
                CoreAPI.Log.LogWarning("[NetworkRouter] ZRoutedRpc not yet available.");
        }

        /// <summary>
        /// Subscribe to a specific network event type.
        /// </summary>
        public void Subscribe<T>(Action<T> handler) where T : INetworkEvent, new()
        {
            var type = typeof(T);
            if (_handlers.ContainsKey(type))
            {
                CoreAPI.Log.LogWarning($"[NetworkRouter] Handler for {type.Name} already registered.");
                return;
            }

            // Register local handler
            _handlers[type] = e => handler((T)e);

            // Register network RPC
            if (ZRoutedRpc.instance != null)
            {
                string rpcName = _rpcBase + type.Name;

                ZRoutedRpc.instance.Register<ZPackage>(rpcName, (long senderId, ZPackage pkg) =>
                {
                    try
                    {
                        var ev = new T();
                        ev.Deserialize(pkg);
                        _handlers[type]?.Invoke(ev);
                    }
                    catch (Exception ex)
                    {
                        CoreAPI.Log.LogError($"[NetworkRouter] Error receiving {type.Name}: {ex}");
                    }
                });

                CoreAPI.Log.LogInfo($"[NetworkRouter] Registered RPC for {type.Name}");
            }
        }

        /// <summary>
        /// Publish a network event to all clients (or a specific target).
        /// </summary>
        public void Publish<T>(T ev, long targetId = 0) where T : INetworkEvent
        {
            if (ZRoutedRpc.instance == null)
            {
                CoreAPI.Log.LogWarning("[NetworkRouter] Tried to send event but ZRoutedRpc is null.");
                return;
            }

            string rpcName = _rpcBase + ev.GetType().Name;

            var pkg = new ZPackage();
            ev.Serialize(pkg);

            if (targetId == 0)
                ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.Everybody, rpcName, pkg);
            else
                ZRoutedRpc.instance.InvokeRoutedRPC(targetId, rpcName, pkg);
        }
    }
}
