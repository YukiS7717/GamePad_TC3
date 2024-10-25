using System;
using System.Collections.Generic;
using SharpDX.DirectInput;
using TwinCATUsbControllerApp.Models;

namespace TwinCATUsbControllerApp.Mappings
{
    public class DualSenseControllerMapping : IControllerMapping
    {
        public string Name => "DualSense";
        public bool SupportsGyro => true;
        public float GyroSensitivity { get; set; } = 1.0f;

        private readonly ConfigurationManager configManager;

        public DualSenseControllerMapping(ConfigurationManager config)
        {
            configManager = config;
            GyroSensitivity = config.GyroSensitivity;
        }

        public Dictionary<string, int> GetButtonMapping()
        {
            return new Dictionary<string, int>
            {
                {"A", 1},  // ×ボタン
                {"B", 2},  // ○ボタン
                {"X", 0},  // □ボタン
                {"Y", 3},  // △ボタン
                {"L1", 4}, // L1ボタン
                {"R1", 5}, // R1ボタン
                {"L2D", 6}, // L2ボタン
                {"R2D", 7}, // R2ボタン
                {"L3", 10}, // L3ボタン（左スティック押し込み）
                {"R3", 11}, // R3ボタン（右スティック押し込み）
                {"Start", 8},  // OPTIONSボタン
                {"Select", 9}, // CREATEボタン
                {"PS", 12},   // PSボタン
                {"TouchPad", 13} // タッチパッドクリック
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
                {"TriggerR2", value => (short)(value)},
                {"GyroX", value => ApplyGyroSettings(value, "X")},
                {"GyroY", value => ApplyGyroSettings(value, "Y")},
                {"GyroZ", value => ApplyGyroSettings(value, "Z")}
            };
        }

        private short ApplyGyroSettings(int value, string axis)
        {
            if (!configManager.GyroEnabled)
                return 0;

            float sensitivity = configManager.GyroSensitivity;
            float normalizedValue = (value - 32768) * sensitivity;

            // 軸の反転設定を適用
            switch (axis)
            {
                case "X":
                    if (configManager.GyroInvertX)
                        normalizedValue = -normalizedValue;
                    break;
                case "Y":
                    if (configManager.GyroInvertY)
                        normalizedValue = -normalizedValue;
                    break;
                case "Z":
                    if (configManager.GyroInvertZ)
                        normalizedValue = -normalizedValue;
                    break;
            }

            // デッドゾーンの適用
            const float deadzone = 0.1f;
            if (Math.Abs(normalizedValue) < deadzone * 32768)
                return 0;

            return (short)Math.Clamp(normalizedValue, -32768, 32767);
        }

        public ST_GamePad MapState(JoystickState state)
        {
            var buttonMapping = GetButtonMapping();
            var axisMapping = GetAxisMapping();

            // ジャイロの値を取得
            short gyroX = axisMapping["GyroX"](state.AccelerationX);
            short gyroY = axisMapping["GyroY"](state.AccelerationY);
            short gyroZ = axisMapping["GyroZ"](state.AccelerationZ);

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
                // ジャイロセンサーの値を設定
                GyroX = gyroX,
                GyroY = gyroY,
                GyroZ = gyroZ,
                GyroEnabled = configManager.GyroEnabled
            };
        }
    }
}