using System;
using System.IO;
using System.Text.Json;
using BepInEx;
using UltimaValheim.Core;

namespace UltimaValheim.Skills
{
    public static class PlayerSkillPersistence
    {
        private static readonly string SaveDir = Path.Combine(Paths.ConfigPath, "UltimaValheim", "Skills");

        public static void Save(PlayerSkillData data)
        {
            if (data == null) return;

            try
            {
                Directory.CreateDirectory(SaveDir);
                string path = Path.Combine(SaveDir, $"{data.PlayerID}.json");

                string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(path, json);
            }
            catch (Exception ex)
            {
                CoreAPI.Log.LogWarning($"[UVC Skills] Failed to save skill data: {ex}");
            }
        }

        public static PlayerSkillData Load(long playerID)
        {
            try
            {
                string path = Path.Combine(SaveDir, $"{playerID}.json");
                if (!File.Exists(path)) return null;

                string json = File.ReadAllText(path);
                return JsonSerializer.Deserialize<PlayerSkillData>(json);
            }
            catch (Exception ex)
            {
                CoreAPI.Log.LogWarning($"[UVC Skills] Failed to load skill data: {ex}");
                return null;
            }
        }

        public static string Serialize(PlayerSkillData data)
        {
            try
            {
                return JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            }
            catch (Exception ex)
            {
                CoreAPI.Log.LogWarning($"[UVC Skills] Serialize failed: {ex}");
                return "{}";
            }
        }

        public static PlayerSkillData Deserialize(string json)
        {
            try
            {
                return string.IsNullOrEmpty(json) ? null : JsonSerializer.Deserialize<PlayerSkillData>(json);
            }
            catch (Exception ex)
            {
                CoreAPI.Log.LogWarning($"[UVC Skills] Deserialize failed: {ex}");
                return null;
            }
        }
    }
}
