using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using UltimaValheim.Core;

namespace UltimaValheim.Skills
{
    /// <summary>
    /// Compatibility patch for when a new player joins the world.
    /// Works across Valheim versions by reflecting connection methods dynamically.
    /// </summary>
    [HarmonyPatch]
    public static class PlayerSkillHooks
    {
        // Harmony will determine which function to patch at runtime.
        static MethodBase TargetMethod()
        {
            var znetType = typeof(ZNet);
            return
                znetType.GetMethod("RPC_PeerInfo", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                ?? znetType.GetMethod("RPC_CharacterID", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                ?? znetType.GetMethod("RPC_ServerHandshake", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                ?? throw new MissingMethodException("No supported connection RPC found in ZNet.");
        }

        [HarmonyPostfix]
        public static void Postfix(object __instance, ZRpc rpc)
        {
            try
            {
                if (ZNet.instance == null)
                {
                    CoreAPI.Log.LogWarning("[UVC Skills] ZNet.instance is null during connection hook.");
                    return;
                }

                var peer = ZNet.instance.GetPeer(rpc);
                if (peer == null)
                {
                    CoreAPI.Log.LogWarning("[UVC Skills] Could not resolve ZNetPeer from ZRpc.");
                    return;
                }

                CoreAPI.Log.LogInfo($"[UVC Skills] Player connected: {peer.m_playerName} ({peer.m_uid})");
                PlayerSkillManager.OnPlayerJoin(peer);
            }
            catch (Exception ex)
            {
                CoreAPI.Log.LogError($"[UVC Skills] Error in player connection hook: {ex}");
            }
        }
    }

    [HarmonyPatch(typeof(ZNet), nameof(ZNet.Disconnect))]
    public static class ZNet_Disconnect_Patch
    {
        [HarmonyPrefix]
        public static void Prefix(ZNetPeer peer)
        {
            if (peer == null) return;

            CoreAPI.Log.LogInfo($"[UVC Skills] Player disconnected: {peer.m_playerName}");
            PlayerSkillManager.OnPlayerLeave(peer);
        }
    }
}
