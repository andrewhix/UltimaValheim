using System.IO;
using UltimaValheim.Core;
using Newtonsoft.Json;

namespace UltimaValheim.Skills
{
    public static class PlayerSkillPersistence
    {
        private static string SaveDir => Path.Combine(CoreAPI.Persistence.SavePath, "Skills");

        public static PlayerSkillData Load(long playerID)
        {
            try
            {
                if (!Directory.Exists(SaveDir))
                    Directory.CreateDirectory(SaveDir);

                string path = Path.Combine(SaveDir, $"{playerID}.json");
                if (!File.Exists(path))
                {
                    CoreAPI.Log.LogInfo($"[UVC Skills] No save found for player {playerID}, creating new profile.");
                    return null;
                }

                string json = File.ReadAllText(path);
                var data = JsonConvert.DeserializeObject<PlayerSkillData>(json);
                CoreAPI.Log.LogInfo($"[UVC Skills] Loaded skill data for player {playerID}");
                return data;
            }
            catch (System.Exception ex)
            {
                CoreAPI.Log.LogError($"[UVC Skills] Failed to load player skill data: {ex.Message}");
                return null;
            }
        }

        public static void Save(PlayerSkillData data)
        {
            try
            {
                if (!Directory.Exists(SaveDir))
                    Directory.CreateDirectory(SaveDir);

                string path = Path.Combine(SaveDir, $"{data.PlayerID}.json");
                string json = JsonConvert.SerializeObject(data, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(path, json);
                CoreAPI.Log.LogInfo($"[UVC Skills] Saved skill data for player {data.PlayerID}");
            }
            catch (System.Exception ex)
            {
                CoreAPI.Log.LogError($"[UVC Skills] Failed to save player skill data: {ex.Message}");
            }
        }
    }
}
