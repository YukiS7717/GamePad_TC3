using System.Runtime.InteropServices;

namespace TwinCATUsbControllerApp
{
    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public class ST_GamePad
    {
        public short LeftStickX;
        public short LeftStickY;
        public short RightStickX;
        public short RightStickY;
        [MarshalAs(UnmanagedType.I1)] public bool ButtonA;
        [MarshalAs(UnmanagedType.I1)] public bool ButtonB;
        [MarshalAs(UnmanagedType.I1)] public bool ButtonX;
        [MarshalAs(UnmanagedType.I1)] public bool ButtonY;
        [MarshalAs(UnmanagedType.I1)] public bool ButtonL1;
        [MarshalAs(UnmanagedType.I1)] public bool ButtonR1;
        public ushort TriggerL2;
        public ushort TriggerR2;
        [MarshalAs(UnmanagedType.I1)] public bool ButtonL2D;
        [MarshalAs(UnmanagedType.I1)] public bool ButtonR2D;
        [MarshalAs(UnmanagedType.I1)] public bool ButtonL3;
        [MarshalAs(UnmanagedType.I1)] public bool ButtonR3;
        [MarshalAs(UnmanagedType.I1)] public bool ButtonStart;
        [MarshalAs(UnmanagedType.I1)] public bool ButtonSelect;
        [MarshalAs(UnmanagedType.I1)] public bool DPadUp;
        [MarshalAs(UnmanagedType.I1)] public bool DPadDown;
        [MarshalAs(UnmanagedType.I1)] public bool DPadLeft;
        [MarshalAs(UnmanagedType.I1)] public bool DPadRight;

        public override string ToString()
        {
            return $"LeftStick: ({LeftStickX}, {LeftStickY}), RightStick: ({RightStickX}, {RightStickY})\n" +
                   $"A: {ButtonA}, B: {ButtonB}, X: {ButtonX}, Y: {ButtonY}\n" +
                   $"L1: {ButtonL1}, R1: {ButtonR1}, L2: {TriggerL2}, R2: {TriggerR2}\n" +
                   $"L2D: {ButtonL2D}, R2D: {ButtonR2D}, L3: {ButtonL3}, R3: {ButtonR3}\n" +
                   $"Start: {ButtonStart}, Select: {ButtonSelect}\n" +
                   $"DPad: Up: {DPadUp}, Down: {DPadDown}, Left: {DPadLeft}, Right: {DPadRight}";
        }
    }
}