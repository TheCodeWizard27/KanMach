using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanMach.Veldrid.Input
{
    [Flags]
    public enum GamepadButton
    {
        DPadUp = 1,
        DPadDown = 2,
        DPadLeft = 4,
        DPadRight = 8,
        Start = 16,
        Back = 32,
        LeftStick = 64,
        RightStick = 128,
        LeftShoulder = 256,
        RightShoulder = 512,
        BigButton = 2048,
        A = 4096,
        B = 8192,
        X = 16384,
        Y = 32768,
        LeftThumbstickLeft = 2097152,
        RightTrigger = 4194304,
        LeftTrigger = 8388608,
        RightThumbstickUp = 16777216,
        RightThumbstickDown = 33554432,
        RightThumbstickRight = 67108864,
        RightThumbstickLeft = 134217728,
        LeftThumbstickUp = 268435456,
        LeftThumbstickDown = 536870912,
        LeftThumbstickRight = 1073741824
    }
}
