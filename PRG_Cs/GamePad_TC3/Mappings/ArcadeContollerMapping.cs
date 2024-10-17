using System;
using System.Collections.Generic;
using SharpDX.DirectInput;
using TwinCATUsbControllerApp.Models;

namespace TwinCATUsbControllerApp.Mappings
{
    public class ArcadeControllerMapping : IControllerMapping
    {
        public string Name => "Arcade";

        public Dictionary<string, int> GetButtonMapping()
        {
            return new Dictionary<string, int>
            {
                {"A", 0}, 
                {"B", 1}, 
                {"X", 2}, 
                {"Y", 3},
                {"L1", 4}, 
                {"R1", 5}, 
                {"L2D", 6}, 
                {"R2D", 7},
                {"Select", 8}, 
                {"Start", 9},
                {"L3", 10},
                {"R3", 11},
            };
        }

        public Dictionary<string, Func<int, short>> GetAxisMapping()
        {
            return new Dictionary<string, Func<int, short>>
            {
                {"LeftStickX", value => (short)(value - 32768)},
                {"LeftStickY", value => (short)(32767 - value)},
                {"RightStickX", value => 0},
                {"RightStickY", value => 0},
                {"TriggerL2", value => 0},
                {"TriggerR2", value => 0}
            };
        }

        public ST_GamePad MapState(JoystickState state)
        {
            var buttonMapping = GetButtonMapping();
            var axisMapping = GetAxisMapping();

            return new ST_GamePad
            {
                LeftStickX = axisMapping["LeftStickX"](state.X),
                LeftStickY = axisMapping["LeftStickY"](state.Y),
                RightStickX = 0,
                RightStickY = 0,
                ButtonA = state.Buttons[buttonMapping["A"]],
                ButtonB = state.Buttons[buttonMapping["B"]],
                ButtonX = state.Buttons[buttonMapping["X"]],
                ButtonY = state.Buttons[buttonMapping["Y"]],
                ButtonL1 = state.Buttons[buttonMapping["L1"]],
                ButtonR1 = state.Buttons[buttonMapping["R1"]],
                TriggerL2 = 0,
                TriggerR2 = 0,
                ButtonL2D = state.Buttons[buttonMapping["L2D"]],
                ButtonR2D = state.Buttons[buttonMapping["R2D"]],
                ButtonL3 = state.Buttons[buttonMapping["L3"]],
                ButtonR3 = state.Buttons[buttonMapping["R3"]],
                ButtonStart = state.Buttons[buttonMapping["Start"]],
                ButtonSelect = state.Buttons[buttonMapping["Select"]],
                DPadUp = state.PointOfViewControllers[0] == 0,
                DPadRight = state.PointOfViewControllers[0] == 9000,
                DPadDown = state.PointOfViewControllers[0] == 18000,
                DPadLeft = state.PointOfViewControllers[0] == 27000,
            };
        }
    }
}