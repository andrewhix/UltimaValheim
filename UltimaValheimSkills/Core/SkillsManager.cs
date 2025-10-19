using UltimaValheim.Core;

namespace UltimaValheim.Skills
{
    public static class SkillsManager
    {
        public static void OnXPEvent(long playerID, string skillName, float amount)
        {
            PlayerSkillManager.GrantXP(playerID, skillName, amount);
        }

        public static void DebugAddXP()
        {
            var player = Player.m_localPlayer;
            if (player != null)
            {
                PlayerSkillManager.GrantXP(player.GetPlayerID(), "Mining", 25f);
            }
        }
    }
}
