using BepInEx.Logging;
using HarmonyLib;
using UltimaValheim.Core;
using System;
using System.Reflection;

namespace UltimaValheim.Skills
{
    [HarmonyPatch]
    public static class PlayerSkillHooks
    {
        private static ManualLogSource Log => CoreAPI.Log;

        [HarmonyPatch(typeof(Player), nameof(Player.OnSpawned))]
        [HarmonyPostfix]
        public static void OnPlayerSpawned(Player __instance)
        {
            try
            {
                if (!ZNet.instance) return;

                long playerID = GetSafePlayerID(__instance);
                Log.LogInfo($"[UVC Skills] Player spawned. Loading skills for ID {playerID}...");
                PlayerSkillManager.LoadPlayer(playerID);
            }
            catch (Exception ex)
            {
                Log.LogError($"[UVC Skills] Exception in OnPlayerSpawned: {ex}");
            }
        }

        [HarmonyPatch(typeof(ZNet), nameof(ZNet.Disconnect))]
        [HarmonyPrefix]
        public static void OnPlayerDisconnect(ZNet __instance, ZNetPeer peer)
        {
            try
            {
                if (peer?.m_uid == 0) return;

                Log.LogInfo($"[UVC Skills] Player disconnect detected for {peer.m_uid}. Saving skill data...");
                PlayerSkillManager.SavePlayer(peer.m_uid);
            }
            catch (Exception ex)
            {
                Log.LogError($"[UVC Skills] Exception in OnPlayerDisconnect: {ex}");
            }
        }

        private static long GetSafePlayerID(Player player)
        {
            try
            {
                // Use ServerCharacters if available
                Type scType = Type.GetType("ServerCharacters.ServerCharacters, ServerCharacters");
                if (scType != null)
                {
                    var getUID = scType.GetMethod("GetPlayerUID", BindingFlags.Public | BindingFlags.Static);
                    if (getUID != null)
                    {
                        object uid = getUID.Invoke(null, new object[] { player });
                        if (uid is long scId)
                        {
                            return scId;
                        }
                    }
                }

                // Access Character.m_nview via reflection to get ZDO
                var nviewField = typeof(Character).GetField("m_nview", BindingFlags.Instance | BindingFlags.NonPublic);
                var nview = nviewField?.GetValue(player) as ZNetView;
                var zdo = nview?.GetZDO();
                if (zdo != null)
                {
                    return zdo.m_uid.UserID;
                }

                // Fallback via ZNetPeer
                var peer = ZNet.instance?.GetPeer(player?.GetPlayerID());
                if (peer != null)
                {
                    return peer.m_uid;
                }

                // Singleplayer fallback
                return ZNet.GetUID();
            }
            catch (Exception ex)
            {
                Log.LogWarning($"[UVC Skills] GetSafePlayerID fallback: {ex}");
                return 0L;
            }
        }
    }
}
