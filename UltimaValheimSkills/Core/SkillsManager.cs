using UltimaValheim.Core;
using UltimaValheim.Skills.core;

namespace UltimaValheim.Skills
{
    /// <summary>
    /// Handles high-level skill registration, XP application, and event forwarding.
    /// </summary>
    public static class SkillsManager
    {
        /// <summary>
        /// Called by gameplay systems to grant XP to a player for a given skill.
        /// </summary>
        public static void OnXPEvent(long playerID, string skillName, float amount)
        {
            if (amount <= 0f) return;
            PlayerSkillManager.GrantXP(playerID, skillName, amount);
        }

        /// <summary>
        /// Debug helper — adds test XP to local player.
        /// </summary>
        public static void DebugAddXP()
        {
            var player = Player.m_localPlayer;
            if (player != null)
            {
                PlayerSkillManager.GrantXP(player.GetPlayerID(), "Mining", 25f);
                CoreAPI.Log.LogInfo($"[UVC Skills] Granted 25 XP to Mining for {player.GetPlayerName()}.");
            }
        }
    }
}
