using System.Collections.Generic;

namespace TwinCATUsbControllerApp.Mappings
{
    public class ControllerMappings
    {
        private Dictionary<string, IControllerMapping> mappings;

        public ControllerMappings()
        {
            InitializeMappings();
        }

        private void InitializeMappings()
        {
            mappings = new Dictionary<string, IControllerMapping>
            {
                { "Xbox", new XboxControllerMapping() },
                { "PlayStation", new PlayStationControllerMapping() },
                { "Arcade", new ArcadeControllerMapping() }
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