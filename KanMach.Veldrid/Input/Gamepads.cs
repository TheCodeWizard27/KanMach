using KanMach.Veldrid.Input.SDL_Mapping;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Veldrid.Sdl2;

namespace KanMach.Veldrid.Input
{
    public class Gamepads : IEnumerable<Gamepad>
    {
        internal static readonly UInt32 SDL_INIT_JOYSTICK = 0x00000200u;

        private static bool _initialised;

        private Dictionary<SDL_JoystickGUID, Gamepad> _connectedGamepads;

        #region SDL2 imports

        [DllImport("SDL2", CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_NumJoysticks();

        [DllImport("SDL2", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool SDL_IsGameController(int index);

        [DllImport("SDL2", CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_InitSubSystem(UInt32 flags);

        [DllImport("SDL2", CallingConvention = CallingConvention.Cdecl)]
        private static extern SDL_GameController SDL_GameControllerOpen(int joystick_index);

        [DllImport("SDL2", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr SDL_JoystickOpen(int joystick_index);

        [DllImport("SDL2", CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_GameControllerClose(SDL_GameController gamecontroller);

        [DllImport("SDL2", CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_JoystickClose(IntPtr joystickGuid);

        [DllImport("SDL2", CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_GameControllerAddMappingsFromRW(IntPtr rw);

        [DllImport("SDL2", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr SDL_GameControllerMapping(SDL_GameController gamecontroller);

        [DllImport("SDL2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr SDL_RWFromFile(string fileName, string mode);

        [DllImport("SDL2", CallingConvention = CallingConvention.Cdecl)]
        private static extern SDL_JoystickGUID SDL_JoystickGetDeviceGUID(int index);

        [DllImport("SDL2")]
        private extern static void SDL_JoystickUpdate();

        #endregion

        public delegate void OnConnectionHandler(Gamepad connectedGamepad);
        public event OnConnectionHandler OnConnect;
        public event OnConnectionHandler OnDisconnect;

        public delegate void OnButtonEventHandler(Gamepad gamepad, GamepadButton button);
        public event OnButtonEventHandler OnButtonDown;
        public event OnButtonEventHandler OnButtonUp;
        public event OnButtonEventHandler OnButtonPressed;
        public event OnButtonEventHandler OnButtonClicked;
        public event OnButtonEventHandler OnButtonReleased;

        internal void Init()
        {
            if (_initialised) return;
            _initialised = true;

            var gameControllerDbPath = "Input/GameControllerDb.txt";
            _connectedGamepads = new Dictionary<SDL_JoystickGUID, Gamepad>(new SDLJoystickGuidComparer());

            SDL_InitSubSystem(SDL_INIT_JOYSTICK);
            var result = SDL_GameControllerAddMappingsFromRW(SDL_RWFromFile(gameControllerDbPath, "r"));
            if (result < 0)
            {
                throw new Exception("Could not load Controller Mappings");
            }
        }

        internal void Poll()
        {
            try
            {
                var joyStickCount = SDL_NumJoysticks();
                SDL_JoystickUpdate();

                for (var i = 0; i < joyStickCount; i++)
                {
                    if (!SDL_IsGameController(i)) continue;

                    var joystickGuid = SDL_JoystickGetDeviceGUID(i);

                    var gamepad = _connectedGamepads.GetValueOrDefault(joystickGuid);
                    if (gamepad != null) continue;

                    var handle = SDL_JoystickOpen(i);
                    if (handle == IntPtr.Zero) continue;

                    var gameController = SDL_GameControllerOpen(i);

                    var mapping = Marshal.PtrToStringAnsi(SDL_GameControllerMapping(gameController));
                    var mapper = new GamepadMap(mapping);
                    gamepad = new Gamepad(handle, mapper);
                    _connectedGamepads.Add(joystickGuid, gamepad);
                    ConnectGamepadEvents(gamepad);

                    SDL_GameControllerClose(gameController);
                }

                foreach(var gamepadEntry in _connectedGamepads.ToList())
                {
                    if(!gamepadEntry.Value.IsConnected)
                    {
                        _connectedGamepads.Remove(gamepadEntry.Key);
                        OnDisconnect?.Invoke(gamepadEntry.Value);

                        SDL_JoystickClose(gamepadEntry.Value.Handle);
                        continue;
                    }

                    gamepadEntry.Value.Poll();
                }

            }catch
            {
                Console.WriteLine("Caught Exception while polling gamepad state");
            }
        }

        private void ConnectGamepadEvents(Gamepad gamepad)
        {
            OnConnect?.Invoke(gamepad);
            gamepad.OnButtonDown += (button) => OnButtonDown?.Invoke(gamepad, button);
            gamepad.OnButtonUp += (button) => OnButtonDown?.Invoke(gamepad, button);
            gamepad.OnButtonPressed += (button) => OnButtonDown?.Invoke(gamepad, button);
            gamepad.OnButtonClicked += (button) => OnButtonDown?.Invoke(gamepad, button);
            gamepad.OnButtonReleased += (button) => OnButtonDown?.Invoke(gamepad, button);

        }

        public IEnumerator<Gamepad> GetEnumerator()
        {
            return this._connectedGamepads.Select(x => x.Value).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
