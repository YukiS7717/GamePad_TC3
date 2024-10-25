using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;
using TwinCATUsbControllerApp.Mappings;
using TwinCATUsbControllerApp.Models;
using TwinCATUsbControllerApp.Utilities;

namespace TwinCATUsbControllerApp.Controllers
{
    public class ControllerManager : IDisposable
    {
        private DirectInput directInput;
        private Dictionary<int, Joystick> joysticks;
        private List<DeviceInstance> availableControllers;
        private ControllerMappings mappings;
        private Dictionary<int, string> controllerMappings;
        private readonly ConfigurationManager configManager;

        public ControllerManager(ConfigurationManager config)
        {
            configManager = config;
            directInput = new DirectInput();
            joysticks = new Dictionary<int, Joystick>();
            mappings = new ControllerMappings(config);  // configを渡す
            controllerMappings = new Dictionary<int, string>
        {
            { 1, config.Controller1Mapping },
            { 2, config.Controller2Mapping }
        };
        }

        public List<string> RefreshControllerList()
        {
            availableControllers = directInput.GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AllDevices).ToList();
            return availableControllers.Select(d => d.InstanceName).ToList();
        }

        public bool ConnectController(int controllerId, int index)
        {
            try
            {
                if (joysticks.ContainsKey(controllerId))
                {
                    joysticks[controllerId].Unacquire();
                    joysticks[controllerId].Dispose();
                }

                var joystick = new Joystick(directInput, availableControllers[index].InstanceGuid);
                joystick.Acquire();
                joysticks[controllerId] = joystick;
                Logger.Log($"Controller {controllerId} connected successfully. Name: {joystick.Information.ProductName}");
                return true;
            }
            catch (Exception ex)
            {
                Logger.Log($"Error connecting controller {controllerId}: {ex.Message}");
                return false;
            }
        }

        public void SetControllerMapping(int controllerId, string mappingName)
        {
            controllerMappings[controllerId] = mappingName;
            Logger.Log($"Controller {controllerId} mapping set to {mappingName}");
        }

        public string GetControllerName(int controllerId)
        {
            return joysticks.ContainsKey(controllerId) ? joysticks[controllerId].Information.ProductName : string.Empty;
        }

        public List<string> GetAvailableMappings()
        {
            return mappings.GetAvailableMappings();
        }

        public ST_GamePad GetControllerState(int controllerId)
        {
            if (!joysticks.ContainsKey(controllerId)) return null;

            var joystick = joysticks[controllerId];
            joystick.Poll();
            var state = joystick.GetCurrentState();

            var mapping = mappings.GetMapping(controllerMappings[controllerId]);
            return mapping.MapState(state);
        }

        public void Dispose()
        {
            foreach (var joystick in joysticks.Values)
            {
                joystick.Unacquire();
                joystick.Dispose();
            }
            directInput.Dispose();
        }
    }
}