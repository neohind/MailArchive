using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MailArchive
{
    internal class Settings
    {
        static public Settings Instance { get; } = Load();
        
        public string Pop3Server { get; set; } = "";

        public int Pop3Port { get; set; } = 0;

        public bool Pop3UseSsl { get; set; } = true;

        public string Pop3User { get; set; } = "";

        public string Pop3Password { get; set; } = "";

        [JsonIgnore]
        public bool IsSucess { get; private set; } = false;

        
        private static string ConfigFilePath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "settings.json");

        private static Settings Load()
        {
            ConfigFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json");

            if (File.Exists(ConfigFilePath))
            {
                var json = File.ReadAllText(ConfigFilePath);
                Settings? settings = null;

                try
                {
                    settings = JsonSerializer.Deserialize<Settings>(json);
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Error loading settings: {ex.Message}");
                }

                if (settings != null)
                {
                    if(!string.IsNullOrEmpty(settings.Pop3Server) 
                        && settings.Pop3Port > 0 
                        && !string.IsNullOrEmpty(settings.Pop3User) 
                        && !string.IsNullOrEmpty(settings.Pop3Password))
                    {
                        settings.IsSucess = true;
                        return settings;
                    }
                }
                settings.IsSucess = false;
            }
            return new Settings();
        }

        public void Save()
        {
            var json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(ConfigFilePath, json);
        }
    }
}
