using System;
using System.Collections.Generic;
using SharpDX.DirectInput;
using TwinCATUsbControllerApp.Models;

namespace TwinCATUsbControllerApp.Mappings
{
    public class PlayStationControllerMapping : IControllerMapping
    {
        public string Name => "PlayStation";
        public bool SupportsGyro => false;
        public float GyroSensitivity { get; set; } = 0.0f;

        private readonly ConfigurationManager configManager;

        public PlayStationControllerMapping(ConfigurationManager config)
        {
            configManager = config;
        }

        public Dictionary<string, int> GetButtonMapping()
        {
            return new Dictionary<string, int>
            {
                {"A", 1},  // ×ボタン
                {"B", 2},  // ○ボタン
                {"X", 0},  // □ボタン
                {"Y", 3},  // △ボタン
                {"L1", 4},
                {"R1", 5},
                {"L2D", 6},
                {"R2D", 7},
                {"L3", 10},
                {"R3", 11},
                {"Start", 8},  // OPTIONSボタン
                {"Select", 9}  // SHAREボタン
            };
        }

        public Dictionary<string, Func<int, short>> GetAxisMapping()
        {
            return new Dictionary<string, Func<int, short>>
            {
                {"LeftStickX", value => (short)(value - 32768)},
                {"LeftStickY", value => (short)(32767 - value)},
                {"RightStickX", value => (short)(value - 32768)},
                {"RightStickY", value => (short)(32767 - value)},
                {"TriggerL2", value => (short)(value)},
                {"TriggerR2", value => (short)(value)}
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
                RightStickX = axisMapping["RightStickX"](state.Z),
                RightStickY = axisMapping["RightStickY"](state.RotationZ),
                ButtonA = state.Buttons[buttonMapping["A"]],
                ButtonB = state.Buttons[buttonMapping["B"]],
                ButtonX = state.Buttons[buttonMapping["X"]],
                ButtonY = state.Buttons[buttonMapping["Y"]],
                ButtonL1 = state.Buttons[buttonMapping["L1"]],
                ButtonR1 = state.Buttons[buttonMapping["R1"]],
                TriggerL2 = (ushort)state.RotationX,
                TriggerR2 = (ushort)state.RotationY,
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
                // ジャイロはサポートしないので常に0
                GyroX = 0,
                GyroY = 0,
                GyroZ = 0,
                GyroEnabled = false
            };
        }
    }
}