using KanMach.Veldrid.Input.SDL_Mapping;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veldrid.Sdl2;

namespace KanMach.Veldrid.Input
{
    public class Gamepad
    {

        private GamepadState _previousState = new GamepadState();
        private GamepadState _currentState = new GamepadState();

        public GamepadMap GamepadMap { get; set; }
        public IntPtr Handle { get; private set; }

        public Gamepad(IntPtr handle, GamepadMap gamepadMap)
        {
            Handle = handle;
            GamepadMap = gamepadMap;
        }

        public void Poll()
        {
            _previousState = _currentState;
            _currentState = GamepadMap.PollState(Handle);

            Console.WriteLine($"[{string.Join(',', Convert.ToString((int)_currentState.Buttons, 2).Select(x => x.ToString()))}]");
        }

    }
}
