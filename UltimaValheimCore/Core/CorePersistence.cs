using System.IO;
using BepInEx;
using UltimaValheim.Core;

namespace UltimaValheim.Core
{
    public class PersistenceManager
    {
        private readonly string _saveDir;

        public string SavePath => _saveDir; // ✅ Added for modules like Skills to access

        public PersistenceManager()
        {
            _saveDir = Path.Combine(Paths.ConfigPath, "UltimaValheim");
            if (!Directory.Exists(_saveDir))
                Directory.CreateDirectory(_saveDir);
        }

        public string GetPath(string fileName)
        {
            return Path.Combine(_saveDir, fileName);
        }

        public void SaveText(string fileName, string content)
        {
            File.WriteAllText(GetPath(fileName), content);
        }

        public string LoadText(string fileName)
        {
            var file = GetPath(fileName);
            return File.Exists(file) ? File.ReadAllText(file) : string.Empty;
        }
    }
}
