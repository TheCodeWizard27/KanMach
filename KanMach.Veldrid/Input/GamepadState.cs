using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
using System.Threading.Tasks;

namespace KanMach.Veldrid.Input
{
    public struct GamepadState
    {
        public GamepadButton Buttons;
        public float LeftTrigger;
        public float RightTrigger;
        public Vector2 Left;
        public Vector2 Right;
    }
}
