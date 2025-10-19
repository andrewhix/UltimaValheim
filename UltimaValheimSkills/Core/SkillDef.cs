using System;
using System.Collections.Generic;
using UltimaValheim.Core;

namespace UltimaValheim.Skills
{
    /// <summary>
    /// Represents a skill definition, including its name, description, and XP scaling behavior.
    /// </summary>
    public class SkillDef
    {
        public string Name { get; }
        public string Description { get; }
        public float BaseXPRequired { get; }
        public float ScalingFactor { get; }

        public SkillDef(string name, string description, float baseXPRequired = 100f, float scalingFactor = 1.25f)
        {
            Name = name;
            Description = description;
            BaseXPRequired = baseXPRequired;
            ScalingFactor = scalingFactor;
        }

        /// <summary>
        /// Calculates how much XP is needed for the given level.
        /// Example: Level 1 = 100 XP, Level 2 = 125 XP, Level 3 = 156 XP, etc.
        /// </summary>
        public float RequiredXP(int level)
        {
            return (float)(BaseXPRequired * Math.Pow(ScalingFactor, level - 1));
        }

        /// <summary>
        /// Returns a short formatted summary for debugging or UI.
        /// </summary>
        public override string ToString()
        {
            return $"{Name} (Next: {BaseXPRequired} XP x{ScalingFactor} per level)";
        }
    }

    /// <summary>
    /// Central registry for all known skills.
    /// </summary>
    public static class SkillRegistry
    {
        private static readonly Dictionary<string, SkillDef> _skills = new();

        public static void Register(SkillDef def)
        {
            if (!_skills.ContainsKey(def.Name))
            {
                _skills[def.Name] = def;
                CoreAPI.Log.LogInfo($"[UVC Skills] Registered skill: {def.Name}");
            }
        }

        // Legacy compatibility
        public static void RegisterSkill(SkillDef def) => Register(def);
        public static void Clear() => _skills.Clear();

        public static bool Exists(string name) => _skills.ContainsKey(name);

        public static SkillDef Get(string name)
        {
            _skills.TryGetValue(name, out var def);
            return def;
        }

        public static IEnumerable<SkillDef> All => _skills.Values;
    }

}
