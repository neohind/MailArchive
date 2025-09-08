using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MailAchiveUi
{
    internal class Settings
    {
        static public Settings Instance { get; } = Load();

        [JsonPropertyName("IMap4Server")]
        public string MailServer { get; set; } = "";

        [JsonPropertyName("IMap4Port")]
        public int MailServerPort { get; set; } = 0;

        [JsonPropertyName("IMap4UseSsl")]
        public bool MailServerUseSsl { get; set; } = true;

        [JsonPropertyName("IMap4User")]
        public string MailUser { get; set; } = "";

        [JsonPropertyName("IMap4Password")]
        public string MailPassword { get; set; } = "";

        [JsonIgnore]
        public bool IsSucess { get; private set; } = false;

        [JsonPropertyName("ResultPath")]
        public string ResultPath { get; set; } = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "Results");


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
                    if(!string.IsNullOrEmpty(settings.MailServer) 
                        && settings.MailServerPort > 0 
                        && !string.IsNullOrEmpty(settings.MailUser) 
                        && !string.IsNullOrEmpty(settings.MailPassword))
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
