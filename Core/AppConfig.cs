using System;
using System.IO;
using System.Text.Json;

namespace Typemetric.Core
{
    public class AppConfig
    {
        public string DbPath { get; set; }

        public bool RunAtStartup { get; set; } = false;

        private static string ConfigFile => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");

        public static AppConfig Load()
        {
            if (File.Exists(ConfigFile))
            {
                string json = File.ReadAllText(ConfigFile);
                return JsonSerializer.Deserialize<AppConfig>(json) ?? new AppConfig();
            }
            return new AppConfig();
        }

        public bool Save()
        {
            try
            {
                string dir = Path.GetDirectoryName(ConfigFile);
                if (!string.IsNullOrEmpty(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                string json = JsonSerializer.Serialize(this);
                File.WriteAllText(ConfigFile, json);
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
        }
    }
}
