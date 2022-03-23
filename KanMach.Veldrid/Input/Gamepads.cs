using KanMach.Veldrid.Input.SDL_Mapping;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Veldrid.Sdl2;

namespace KanMach.Veldrid.Input
{
    public class Gamepads
    {
        public static readonly UInt32 SDL_INIT_JOYSTICK = 0x00000200u;
        public static readonly UInt32 SDL_INIT_GAMECONTROLLER = 0x00002000u;

        private Dictionary<SDL_JoystickGUID, Gamepad> _connectedGamepads;
        private List<GamepadMap> _mappers;

        #region SDL2 imports

        [DllImport("SDL2", CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_NumJoysticks();

        [DllImport("SDL2", CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_GameControllerUpdate();

        [DllImport("SDL2", CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_JoystickEventState(int state);

        [DllImport("SDL2", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool SDL_IsGameController(int index);

        [DllImport("SDL2", CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_Init(UInt32 flags);

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

        internal void Init()
        {
            var gameControllerDbPath = "Input/GameControllerDb.txt";

            _connectedGamepads = new Dictionary<SDL_JoystickGUID, Gamepad>(new SDLJoystickGuidComparer());
            //_mappers = GamepadMap
            //    .CreateMapping(File.ReadAllText(gameControllerDbPath))
            //    .ToList();

            var joystickInit = SDL_InitSubSystem(SDL_INIT_JOYSTICK);
            SDL_JoystickEventState(1);
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
                SDL_GameController gameController;
                SDL_JoystickGUID joystickGuid;

                SDL_JoystickUpdate();

                for (var i = 0; i < joyStickCount; i++)
                {
                    if (!SDL_IsGameController(i)) continue;

                    gameController = SDL_GameControllerOpen(i);
                    joystickGuid = SDL_JoystickGetDeviceGUID(i);
                    var handle = SDL_JoystickOpen(i);
                    if (handle == IntPtr.Zero)
                        throw new Exception("Zero");

                    var gamepad = _connectedGamepads.GetValueOrDefault(joystickGuid);

                    if (gamepad == null)
                    {
                        var mapping = Marshal.PtrToStringAnsi(SDL_GameControllerMapping(gameController));
                        var mapper = new GamepadMap(mapping);
                        gamepad = new Gamepad(handle, mapper);
                        _connectedGamepads.Add(joystickGuid, gamepad);
                    }

                    gamepad.Poll();

                    //SDL_JoystickClose(handle);
                    //SDL_GameControllerClose(gameController);

                }
            }catch
            {
                Console.WriteLine("Caught Exception while polling gamepad state");
            }
            

            //Console.WriteLine($"[{string.Join(',', _connectedGamepads.Select(x => x.ToString()))}]");
        }

    }
}
