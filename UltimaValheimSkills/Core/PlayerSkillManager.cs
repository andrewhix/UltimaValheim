using System.Collections.Generic;
using UltimaValheim.Core;

namespace UltimaValheim.Skills
{
    public static class PlayerSkillManager
    {
        // Tracks all loaded players and their skill data
        private static readonly Dictionary<long, PlayerSkillData> _playerData = new();

        public static void Initialize()
        {
            CoreAPI.Log.LogInfo("[UVC Skills] PlayerSkillManager initialized.");
        }

        public static void Shutdown()
        {
            CoreAPI.Log.LogInfo("[UVC Skills] Saving all player skill data before shutdown...");
            foreach (var kvp in _playerData)
            {
                PlayerSkillPersistence.Save(kvp.Value);
            }
            _playerData.Clear();
        }

        public static void RegisterLoadedData(PlayerSkillData data)
        {
            if (data == null) return;
            _playerData[data.PlayerID] = data;
            CoreAPI.Log.LogInfo($"[UVC Skills] Registered skill data for player ID {data.PlayerID}");
        }

        public static PlayerSkillData GetData(long playerID)
        {
            if (_playerData.TryGetValue(playerID, out var data))
                return data;

            CoreAPI.Log.LogWarning($"[UVC Skills] No skill data found for player ID {playerID}. Creating new data.");
            var newData = new PlayerSkillData(playerID);
            _playerData[playerID] = newData;
            return newData;
        }

        // 🔹 XP Management API
        public static void GrantXP(long playerID, string skillName, float amount)
        {
            if (!_playerData.TryGetValue(playerID, out var data))
            {
                data = new PlayerSkillData(playerID);
                _playerData[playerID] = data;
            }

            var beforeLevel = data.GetLevel(skillName);
            data.AddXP(skillName, amount);
            var afterLevel = data.GetLevel(skillName);

            CoreAPI.Log.LogInfo($"[UVC Skills] Player {playerID} gained {amount:F1} XP in {skillName} (Lv {beforeLevel} → {afterLevel}).");

            // Optional: save immediately or defer for batch saving later
            PlayerSkillPersistence.Save(data);
        }

        public static void OnPlayerJoin(ZNetPeer peer)
        {
            if (peer == null) return;

            long playerID = peer.m_uid;
            CoreAPI.Log.LogInfo($"[UVC Skills] Player {peer.m_playerName} ({playerID}) joined — loading skills.");

            var data = PlayerSkillPersistence.Load(playerID);
            if (data != null)
            {
                RegisterLoadedData(data);
            }
            else
            {
                CoreAPI.Log.LogInfo($"[UVC Skills] No existing skill data for {peer.m_playerName}. Creating new profile.");
                RegisterLoadedData(new PlayerSkillData(playerID));
            }
        }

        public static void OnPlayerLeave(ZNetPeer peer)
        {
            if (peer == null) return;

            long playerID = peer.m_uid;
            CoreAPI.Log.LogInfo($"[UVC Skills] Player {peer.m_playerName} ({playerID}) left — saving skills.");

            if (_playerData.TryGetValue(playerID, out var data))
            {
                PlayerSkillPersistence.Save(data);
                _playerData.Remove(playerID);
            }
        }

        public static IReadOnlyDictionary<long, PlayerSkillData> All => _playerData;
    }
}
