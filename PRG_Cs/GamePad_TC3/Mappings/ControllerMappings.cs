using System.Collections.Generic;
using TwinCATUsbControllerApp.Models;

namespace TwinCATUsbControllerApp.Mappings
{
    public class ControllerMappings
    {
        private Dictionary<string, IControllerMapping> mappings;
        private readonly ConfigurationManager configManager;

        public ControllerMappings(ConfigurationManager config)
        {
            configManager = config;
            InitializeMappings();
        }

        private void InitializeMappings()
        {
            mappings = new Dictionary<string, IControllerMapping>
            {
                { "Xbox", new XboxControllerMapping(configManager) },
                { "PlayStation", new PlayStationControllerMapping(configManager) },
                { "DualSense", new DualSenseControllerMapping(configManager) },
                { "Arcade", new ArcadeControllerMapping(configManager) }
            };
        }

        public IControllerMapping GetMapping(string mappingName)
        {
            return mappings.ContainsKey(mappingName) ? mappings[mappingName] : mappings["Xbox"];
        }

        public List<string> GetAvailableMappings()
        {
            return new List<string>(mappings.Keys);
        }

        public void AddMapping(IControllerMapping mapping)
        {
            if (!mappings.ContainsKey(mapping.Name))
            {
                mappings.Add(mapping.Name, mapping);
            }
        }
    }
}