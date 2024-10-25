using System.IO;
using System.Xml.Serialization;
using TwinCATUsbControllerApp.Utilities;

namespace TwinCATUsbControllerApp.Models
{
    public class ConfigurationManager
    {
        private const string ConfigFileName = "config.xml";
        public string Controller1Mapping { get; set; } = "Default";
        public string Controller2Mapping { get; set; } = "Default";
        // ジャイロ設定の追加
        public bool GyroEnabled { get; set; } = true;
        public float GyroSensitivity { get; set; } = 1.0f;
        public bool GyroInvertX { get; set; } = false;
        public bool GyroInvertY { get; set; } = false;
        public bool GyroInvertZ { get; set; } = false;

        public void SaveConfiguration()
        {
            try
            {
                using (var writer = new StreamWriter(ConfigFileName))
                {
                    var serializer = new XmlSerializer(typeof(ConfigurationManager));
                    serializer.Serialize(writer, this);
                }
                Logger.Log("Configuration saved successfully.");
            }
            catch (System.Exception ex)
            {
                Logger.Log($"Error saving configuration: {ex.Message}");
            }
        }

        public void LoadConfiguration()
        {
            if (File.Exists(ConfigFileName))
            {
                try
                {
                    using (var reader = new StreamReader(ConfigFileName))
                    {
                        var serializer = new XmlSerializer(typeof(ConfigurationManager));
                        var loadedConfig = (ConfigurationManager)serializer.Deserialize(reader);
                        Controller1Mapping = loadedConfig.Controller1Mapping;
                        Controller2Mapping = loadedConfig.Controller2Mapping;
                    }
                    Logger.Log("Configuration loaded successfully.");
                }
                catch (System.Exception ex)
                {
                    Logger.Log($"Error loading configuration: {ex.Message}");
                }
            }
            else
            {
                Logger.Log("Configuration file not found. Using default settings.");
            }
        }
    }
}