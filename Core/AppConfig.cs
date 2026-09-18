using System;
using System.IO;
using System.Text.Json;

namespace Typemetric.Core
{
    public class AppConfig
    {
        public string DbPath { get; set; }

        private static string ConfigFile => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Typemetric", "config.json");

        public static AppConfig Load()
        {
            if (File.Exists(ConfigFile))
            {
                string json = File.ReadAllText(ConfigFile);
                return JsonSerializer.Deserialize<AppConfig>(json) ?? new AppConfig();
            }
            return new AppConfig();
        }

        public void Save()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(ConfigFile));
            string json = JsonSerializer.Serialize(this);
            File.WriteAllText(ConfigFile, json);
        }
    }
}
