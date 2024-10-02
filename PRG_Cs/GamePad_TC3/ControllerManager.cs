using System;
using System.Collections.Generic;
using System.Linq;
using SharpDX.DirectInput;

namespace TwinCATUsbControllerApp
{
    public class ControllerManager : IDisposable
    {
        private DirectInput directInput;
        private Dictionary<int, Joystick> joysticks;
        private List<DeviceInstance> availableControllers;
        private Dictionary<string, int> buttonMapping;

        public ControllerManager()
        {
            directInput = new DirectInput();
            joysticks = new Dictionary<int, Joystick>();
            InitializeButtonMapping();
        }

        private void InitializeButtonMapping()
        {
            buttonMapping = new Dictionary<string, int>
        {
            {"A", 1},
            {"B", 2},
            {"X", 0},
            {"Y", 3},
            {"L1", 4},
            {"R1", 5},
            {"L2D", 6},
            {"R2D", 7},
            {"L3", 10},
            {"R3", 11},
            {"Start", 8},
            {"Select", 9}
        };
        }

        public List<string> RefreshControllerList()
        {
            availableControllers = directInput.GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AllDevices).ToList();
            Console.WriteLine($"Found {availableControllers.Count} controllers:");
            foreach (var controller in availableControllers)
            {
                Console.WriteLine($"- Name: {controller.InstanceName}");
                Console.WriteLine($"  Type: {controller.Type}");
                Console.WriteLine($"  InstanceGuid: {controller.InstanceGuid}");
                Console.WriteLine($"  ProductGuid: {controller.ProductGuid}");
            }
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

                Console.WriteLine($"Attempting to connect controller {controllerId} at index {index}");
                Console.WriteLine($"Available controllers: {availableControllers.Count}");

                if (index >= 0 && index < availableControllers.Count)
                {
                    var deviceInstance = availableControllers[index];
                    Console.WriteLine($"Selected controller: {deviceInstance.InstanceName}");

                    var joystick = new Joystick(directInput, deviceInstance.InstanceGuid);
                    joystick.Acquire();
                    joysticks[controllerId] = joystick;
                    Console.WriteLine($"Controller {controllerId} connected successfully. Name: {joystick.Information.ProductName}");
                    return true;
                }
                else
                {
                    Console.WriteLine($"Invalid index: {index}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error connecting controller {controllerId}: {ex.Message}");
                return false;
            }
        }

        public bool IsControllerAlreadyConnected(int index)
        {
            var guid = availableControllers[index].InstanceGuid;
            return joysticks.Values.Any(j => j.Information.InstanceGuid == guid);
        }

        public string GetControllerName(int controllerId)
        {
            return joysticks.ContainsKey(controllerId) ? joysticks[controllerId].Information.ProductName : string.Empty;
        }

        public ST_GamePad GetControllerState(int controllerId)
        {
            if (!joysticks.ContainsKey(controllerId)) return null;

            var joystick = joysticks[controllerId];
            joystick.Poll();
            var state = joystick.GetCurrentState();

            return new ST_GamePad
            {
                LeftStickX = (short)(state.X - 32768),
                LeftStickY = (short)(32767 - state.Y), // Y軸を逆転
                RightStickX = (short)(state.Z - 32768),
                RightStickY = (short)(32767 - state.RotationZ), // Y軸を逆転
                ButtonA = state.Buttons[buttonMapping["A"]],
                ButtonB = state.Buttons[buttonMapping["B"]],
                ButtonX = state.Buttons[buttonMapping["X"]],
                ButtonY = state.Buttons[buttonMapping["Y"]],
                ButtonL1 = state.Buttons[buttonMapping["L1"]],
                ButtonR1 = state.Buttons[buttonMapping["R1"]],
                TriggerL2 = (ushort)(state.RotationX), // L2トリガー
                TriggerR2 = (ushort)(state.RotationY), // R2トリガー
                ButtonL2D = state.Buttons[buttonMapping["L2D"]], // L2デジタル
                ButtonR2D = state.Buttons[buttonMapping["R2D"]], // R2デジタル
                ButtonL3 = state.Buttons[buttonMapping["L3"]],
                ButtonR3 = state.Buttons[buttonMapping["R3"]],
                ButtonStart = state.Buttons[buttonMapping["Start"]],
                ButtonSelect = state.Buttons[buttonMapping["Select"]],
                DPadUp = state.PointOfViewControllers[0] == 0,
                DPadRight = state.PointOfViewControllers[0] == 9000,
                DPadDown = state.PointOfViewControllers[0] == 18000,
                DPadLeft = state.PointOfViewControllers[0] == 27000
            };
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