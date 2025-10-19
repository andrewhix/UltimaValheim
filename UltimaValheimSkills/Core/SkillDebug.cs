using System.Linq;
using Jotunn.Entities;
using UltimaValheim.Core;

namespace UltimaValheim.Skills
{
    // Command: addxp <skill> <amount>
    public class AddXPCommand : ConsoleCommand
    {
        public override string Name => "addxp";
        public override string Help => "Usage: addxp <skill> <amount> — Adds XP to the specified skill.";

        public override void Run(string[] args)
        {
            if (args.Length < 2)
            {
                CoreAPI.Log.LogInfo("Usage: addxp <skill> <amount>");
                return;
            }

            string skillName = args[0];
            if (!float.TryParse(args[1], out float amount))
            {
                CoreAPI.Log.LogInfo("Invalid XP amount.");
                return;
            }

            var player = Player.m_localPlayer;
            if (player == null)
            {
                CoreAPI.Log.LogInfo("No local player found.");
                return;
            }

            long id = player.GetPlayerID();
            CoreAPI.Log.LogInfo($"[UVC Skills Debug] Granting {amount} XP to {skillName} for player {player.GetPlayerName()}.");

            PlayerSkillManager.GrantXP(id, skillName, amount);
        }
    }

    // Command: skills.debug
    public class SkillsDebugCommand : ConsoleCommand
    {
        public override string Name => "skills.debug";
        public override string Help => "Displays all skill data for the local player.";

        public override void Run(string[] args)
        {
            var player = Player.m_localPlayer;
            if (player == null)
            {
                CoreAPI.Log.LogInfo("No local player found.");
                return;
            }

            long id = player.GetPlayerID();
            var data = PlayerSkillManager.GetData(id);

            if (data == null)
            {
                CoreAPI.Log.LogInfo("[UVC Skills Debug] No skill data found for this player.");
                return;
            }

            CoreAPI.Log.LogInfo($"[UVC Skills Debug] Skill data for {player.GetPlayerName()}:");
            foreach (var kvp in data.Skills.OrderBy(s => s.Key))
            {
                CoreAPI.Log.LogInfo($"  • {kvp.Key} → {kvp.Value:F2} XP");
            }
        }
    }
}
