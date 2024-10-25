using System;
using System.Collections.Generic;
using SharpDX.DirectInput;
using TwinCATUsbControllerApp.Models;

namespace TwinCATUsbControllerApp.Mappings
{
    public class XboxControllerMapping : IControllerMapping
    {
        public string Name => "Xbox";
        public bool SupportsGyro => false;  // 追加：Xboxコントローラーはジャイロをサポートしない
        public float GyroSensitivity { get; set; } = 0.0f;  // 追加：ジャイロ感度（未使用）

        private readonly ConfigurationManager configManager;  // 追加：設定管理用
        private const int TriggerThreshold = 30000; // L2D/R2Dがtrueになるしきい値

        public XboxControllerMapping(ConfigurationManager config)  // 追加：コンストラクタ
        {
            configManager = config;
        }

        public Dictionary<string, int> GetButtonMapping()
        {
            return new Dictionary<string, int>
            {
                {"A", 0}, {"B", 1}, {"X", 2}, {"Y", 3},
                {"L1", 4}, {"R1", 5}, {"L3", 8}, {"R3", 9},
                {"Start", 7}, {"Select", 6}
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
                {"TriggerL2", value => ExtractL2(value)},
                {"TriggerR2", value => ExtractR2(value)}
            };
        }

        public ST_GamePad MapState(JoystickState state)
        {
            var buttonMapping = GetButtonMapping();
            var axisMapping = GetAxisMapping();

            ushort l2Value = (ushort)axisMapping["TriggerL2"](state.Z);
            ushort r2Value = (ushort)axisMapping["TriggerR2"](state.Z);

            return new ST_GamePad
            {
                LeftStickX = axisMapping["LeftStickX"](state.X),
                LeftStickY = axisMapping["LeftStickY"](state.Y),
                RightStickX = axisMapping["RightStickX"](state.RotationX),
                RightStickY = axisMapping["RightStickY"](state.RotationY),
                ButtonA = state.Buttons[buttonMapping["A"]],
                ButtonB = state.Buttons[buttonMapping["B"]],
                ButtonX = state.Buttons[buttonMapping["X"]],
                ButtonY = state.Buttons[buttonMapping["Y"]],
                ButtonL1 = state.Buttons[buttonMapping["L1"]],
                ButtonR1 = state.Buttons[buttonMapping["R1"]],
                TriggerL2 = l2Value,
                TriggerR2 = r2Value,
                ButtonL2D = l2Value >= TriggerThreshold,
                ButtonR2D = r2Value >= TriggerThreshold,
                ButtonL3 = state.Buttons[buttonMapping["L3"]],
                ButtonR3 = state.Buttons[buttonMapping["R3"]],
                ButtonStart = state.Buttons[buttonMapping["Start"]],
                ButtonSelect = state.Buttons[buttonMapping["Select"]],
                DPadUp = state.PointOfViewControllers[0] == 0,
                DPadRight = state.PointOfViewControllers[0] == 9000,
                DPadDown = state.PointOfViewControllers[0] == 18000,
                DPadLeft = state.PointOfViewControllers[0] == 27000,
                // 追加：ジャイロ関連の値（Xboxコントローラーでは未使用）
                GyroX = 0,
                GyroY = 0,
                GyroZ = 0,
                GyroEnabled = false
            };
        }

        private short ExtractL2(int value)
        {
            // Z軸の後半（32768-65535）をL2トリガーの値として使用
            return (short)Math.Max(0, value - 32768);
        }

        private short ExtractR2(int value)
        {
            // Z軸の前半（0-32767）をR2トリガーの値として使用
            // 値を反転させて、押し込むほど大きくなるようにする
            return (short)(32767 - Math.Min(value, 32767));
        }
    }
}