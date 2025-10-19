using System;
using System.Collections.Generic;
using UnityEngine;

namespace UltimaValheim.Core
{
    /// <summary>
    /// Handles RPC registration and network message dispatching.
    /// </summary>
    public class NetworkManager
    {
        private readonly Dictionary<string, Action<ZRpc, ZPackage>> _rpcHandlers = new();

        public NetworkManager() { }

        public void Register(string name, Action<ZRpc, ZPackage> handler)
        {
            if (_rpcHandlers.ContainsKey(name))
            {
                CoreAPI.Log.LogWarning($"[Network] RPC '{name}' already registered.");
                return;
            }

            _rpcHandlers[name] = handler;

            if (ZRoutedRpc.instance != null)
            {
                // Correct Valheim RPC signature: (long senderId, ZPackage pkg)
                ZRoutedRpc.instance.Register<ZPackage>(name, (long senderId, ZPackage pkg) =>
                {
                    try
                    {
                        ZRpc rpc = ZNet.instance?.GetPeer(senderId)?.m_rpc;
                        handler?.Invoke(rpc, pkg);
                    }
                    catch (Exception ex)
                    {
                        CoreAPI.Log.LogError($"[Network] Error in RPC '{name}': {ex}");
                    }
                });

                CoreAPI.Log.LogInfo($"[Network] Registered RPC: {name}");
            }
            else
            {
                CoreAPI.Log.LogWarning($"[Network] Failed to register RPC '{name}' (ZRoutedRpc not ready)");
            }
        }

        public void Send(string name, ZPackage pkg, long targetId = 0)
        {
            if (ZRoutedRpc.instance == null) return;

            if (targetId == 0)
                ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.Everybody, name, pkg);
            else
                ZRoutedRpc.instance.InvokeRoutedRPC(targetId, name, pkg);
        }
    }
}
