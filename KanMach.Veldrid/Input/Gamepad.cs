using KanMach.Veldrid.Input.SDL_Mapping;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
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

        public bool IsConnected { get => SDL_JoystickGetAttached(Handle); }

        [DllImport("SDL2", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool SDL_JoystickGetAttached(IntPtr joystick);

        public delegate void OnButtonEventHandler(GamepadButton button);
        public event OnButtonEventHandler OnButtonDown;
        public event OnButtonEventHandler OnButtonUp;
        public event OnButtonEventHandler OnButtonPressed;
        public event OnButtonEventHandler OnButtonClicked;
        public event OnButtonEventHandler OnButtonReleased;

        public Vector2 LeftStick { get => _currentState.Left; }
        public Vector2 RightStick { get => _currentState.Right; }
        public float LeftTrigger { get => _currentState.LeftTrigger; }
        public float RightTrigger { get => _currentState.RightTrigger; }

        public Gamepad(IntPtr handle, GamepadMap gamepadMap)
        {
            Handle = handle;
            GamepadMap = gamepadMap;
        }

        public bool IsButtonDown(GamepadButton button)
            => _currentState.IsButtonDown(button);
        public bool IsButtonUp(GamepadButton button)
            => _currentState.IsButtonUp(button);
        public bool IsButtonPressed(GamepadButton button)
            => _previousState.IsButtonDown(button) && _currentState.IsButtonDown(button);
        public bool IsButtonClicked(GamepadButton button)
            => _previousState.IsButtonUp(button) && _currentState.IsButtonDown(button);
        public bool IsButtonReleased(GamepadButton button)
            => _previousState.IsButtonDown(button) && _currentState.IsButtonUp(button);

        internal void Poll()
        {
            _previousState = _currentState;
            _currentState = GamepadMap.PollState(Handle);
            //Console.WriteLine($"DPAD: X[{_currentState.DPad.X}] Y[{_currentState.DPad.X}]");
            //Console.WriteLine($"Left:X[{_currentState.Left.X}] Y[{_currentState.Left.Y}] | Right:X[{_currentState.Right.X}] Y[{_currentState.Right.Y}]");
            //Console.WriteLine($"Left:{_currentState.LeftTrigger} | Right:{_currentState.RightTrigger}");
            //Console.WriteLine($"[{string.Join(',', Convert.ToString((int)_currentState.Buttons, 2).Select(x => x.ToString()))}]");
            InvokeButtonEvents();
        }

        private void InvokeButtonEvents()
        {
            foreach (var button in Enum.GetValues(typeof(GamepadButton)).Cast<GamepadButton>())
            {
                if (IsButtonDown(button))
                {
                    OnButtonDown?.Invoke(button);
                    if (IsButtonPressed(button)) OnButtonPressed?.Invoke(button);
                    if (IsButtonClicked(button)) OnButtonClicked?.Invoke(button);
                }
                else
                {
                    OnButtonUp?.Invoke(button);
                    if (IsButtonReleased(button)) OnButtonReleased?.Invoke(button);
                }
            }
        }

    }
}
