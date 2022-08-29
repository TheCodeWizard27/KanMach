using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Veldrid;

namespace KanMach.Veldrid.Input
{
    public class EmptyInputSnapshot : InputSnapshot
    {
        public IReadOnlyList<KeyEvent> KeyEvents => new List<KeyEvent>();

        public IReadOnlyList<MouseEvent> MouseEvents => new List<MouseEvent>();

        public IReadOnlyList<char> KeyCharPresses => new List<char>();

        public Vector2 MousePosition => Vector2.Zero;

        public float WheelDelta => 0;

        public bool IsMouseDown(MouseButton button)
        {
            return false;
        }
    }
}
