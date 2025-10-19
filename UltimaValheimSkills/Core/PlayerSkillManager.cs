using System;
using UltimaValheim.Core;

namespace UltimaValheim.Skills
{
    public static class PlayerSkillManager
    {
        public static void Initialize()
        {
            CoreAPI.Log.LogInfo("[UVC Skills] PlayerSkillManager initialized.");
        }

        public static void LoadPlayer(long playerID)
        {
            try
            {
                if (Type.GetType("ServerCharacters.ServerCharacters, ServerCharacters") != null)
                {
                    CoreAPI.Log.LogInfo($"[UVC Skills] ServerCharacters active - skipping local skill load for {playerID}");
                    return;
                }

                var data = PlayerSkillPersistence.Load(playerID);
                if (data != null)
                {
                    ApplySkillData(playerID, data);
                    CoreAPI.Log.LogInfo($"[UVC Skills] Skills loaded for player {playerID}");
                }
                else
                {
                    CoreAPI.Log.LogInfo($"[UVC Skills] No local skill data found for player {playerID}");
                }
            }
            catch (Exception ex)
            {
                CoreAPI.Log.LogError($"[UVC Skills] LoadPlayer error for {playerID}: {ex}");
            }
        }

        public static void SavePlayer(long playerID)
        {
            try
            {
                if (Type.GetType("ServerCharacters.ServerCharacters, ServerCharacters") != null)
                {
                    CoreAPI.Log.LogInfo($"[UVC Skills] ServerCharacters active - skipping local skill save for {playerID}");
                    return;
                }

                var data = CollectSkillData(playerID);
                if (data != null)
                {
                    PlayerSkillPersistence.Save(data);
                    CoreAPI.Log.LogInfo($"[UVC Skills] Skills saved for player {playerID}");
                }
            }
            catch (Exception ex)
            {
                CoreAPI.Log.LogError($"[UVC Skills] SavePlayer error for {playerID}: {ex}");
            }
        }

        private static PlayerSkillData CollectSkillData(long playerID)
        {
            // Placeholder for collecting player skill data.
            return new PlayerSkillData(playerID);
        }

        private static void ApplySkillData(long playerID, PlayerSkillData data)
        {
            // Placeholder for applying player skill data.
        }
    }
}
