using System.Collections.Generic;
using UltimaValheim.Core;

namespace UltimaValheim.Skills
{
    public class PlayerSkillData
    {
        public long PlayerID { get; set; }
        public Dictionary<string, SkillProgress> Skills { get; set; } = new();

        public PlayerSkillData(long playerID)
        {
            PlayerID = playerID;
        }

        public void AddXP(string skillName, float amount)
        {
            if (!Skills.TryGetValue(skillName, out var skill))
            {
                skill = new SkillProgress { XP = 0, Level = 1 };
                Skills[skillName] = skill;
            }

            skill.XP += amount;

            float xpToLevel = skill.Level * 100f; // 100 XP per level scaling
            if (skill.XP >= xpToLevel)
            {
                skill.Level++;
                skill.XP -= xpToLevel;
                CoreAPI.Log.LogInfo($"[UVC Skills] {skillName} leveled up! Now level {skill.Level}");
            }
        }

        public int GetLevel(string skillName)
        {
            return Skills.TryGetValue(skillName, out var skill) ? skill.Level : 1;
        }
    }

    public class SkillProgress
    {
        public float XP { get; set; }
        public int Level { get; set; }
    }
}
