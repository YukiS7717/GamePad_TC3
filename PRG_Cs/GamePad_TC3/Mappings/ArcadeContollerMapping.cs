using System;
using System.Collections.Generic;
using SharpDX.DirectInput;
using TwinCATUsbControllerApp.Models;

namespace TwinCATUsbControllerApp.Mappings
{
    public class ArcadeControllerMapping : IControllerMapping
    {
        public string Name => "Arcade";
        public bool SupportsGyro => false;
        public float GyroSensitivity { get; set; } = 0.0f;

        private readonly ConfigurationManager configManager;

        public ArcadeControllerMapping(ConfigurationManager config)
        {
            configManager = config;
        }

        public Dictionary<string, int> GetButtonMapping()
        {
            return new Dictionary<string, int>
            {
                {"A", 0},      // 1Pボタン
                {"B", 1},      // 2Pボタン
                {"X", 2},      // 3Pボタン
                {"Y", 3},      // 4Pボタン
                {"L1", 4},     // 5Pボタン
                {"R1", 5},     // 6Pボタン
                {"L2D", 6},    // 7Pボタン
                {"R2D", 7},    // 8Pボタン
                {"Select", 8}, // コインボタン
                {"Start", 9},  // スタートボタン
                {"L3", 10},    // 未使用
                {"R3", 11}     // 未使用
            };
        }

        public Dictionary<string, Func<int, short>> GetAxisMapping()
        {
            return new Dictionary<string, Func<int, short>>
            {
                {"LeftStickX", value => (short)(value - 32768)},
                {"LeftStickY", value => (short)(32767 - value)},
                {"RightStickX", value => 0},  // アーケードコントローラーには右スティックなし
                {"RightStickY", value => 0},  // アーケードコントローラーには右スティックなし
                {"TriggerL2", value => 0},    // アーケードコントローラーにはアナログトリガーなし
                {"TriggerR2", value => 0},    // アーケードコントローラーにはアナログトリガーなし
                {"GyroX", value => 0},        // アーケードコントローラーにはジャイロなし
                {"GyroY", value => 0},        // アーケードコントローラーにはジャイロなし
                {"GyroZ", value => 0}         // アーケードコントローラーにはジャイロなし
            };
        }

        public ST_GamePad MapState(JoystickState state)
        {
            var buttonMapping = GetButtonMapping();
            var axisMapping = GetAxisMapping();

            // レバーの傾きからD-PADの状態を判定
            bool dpadUp = state.Y < 16384;
            bool dpadDown = state.Y > 49152;
            bool dpadLeft = state.X < 16384;
            bool dpadRight = state.X > 49152;

            return new ST_GamePad
            {
                // レバー（左スティック相当）
                LeftStickX = axisMapping["LeftStickX"](state.X),
                LeftStickY = axisMapping["LeftStickY"](state.Y),

                // 右スティックは常に中立
                RightStickX = 0,
                RightStickY = 0,

                // ボタン類
                ButtonA = state.Buttons[buttonMapping["A"]],
                ButtonB = state.Buttons[buttonMapping["B"]],
                ButtonX = state.Buttons[buttonMapping["X"]],
                ButtonY = state.Buttons[buttonMapping["Y"]],
                ButtonL1 = state.Buttons[buttonMapping["L1"]],
                ButtonR1 = state.Buttons[buttonMapping["R1"]],

                // アナログトリガーは常に0
                TriggerL2 = 0,
                TriggerR2 = 0,

                // デジタルトリガー
                ButtonL2D = state.Buttons[buttonMapping["L2D"]],
                ButtonR2D = state.Buttons[buttonMapping["R2D"]],

                // L3/R3は未使用
                ButtonL3 = false,
                ButtonR3 = false,

                // システムボタン
                ButtonStart = state.Buttons[buttonMapping["Start"]],
                ButtonSelect = state.Buttons[buttonMapping["Select"]],

                // D-PAD（レバーの状態から判定）
                DPadUp = dpadUp,
                DPadRight = dpadRight,
                DPadDown = dpadDown,
                DPadLeft = dpadLeft,

                // ジャイロは常に無効
                GyroX = 0,
                GyroY = 0,
                GyroZ = 0,
                GyroEnabled = false
            };
        }

        // デッドゾーンを適用する補助メソッド
        private bool IsInDeadZone(int value, int center = 32768, int deadZone = 8192)
        {
            return Math.Abs(value - center) < deadZone;
        }

        // 軸の値を正規化する補助メソッド
        private short NormalizeAxis(int value, bool invert = false)
        {
            if (IsInDeadZone(value))
                return 0;

            short normalized = (short)(value - 32768);
            return invert ? (short)-normalized : normalized;
        }
    }
}