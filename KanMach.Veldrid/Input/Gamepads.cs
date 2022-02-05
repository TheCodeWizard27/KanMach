using KanMach.Veldrid.Input.SDL_Mapping;
using System;
using System.Collections.Generic;
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

        private Dictionary<SDL_JoystickGUID, Gamepad> _connectedGamepads;

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
        private static extern SDL_GameController SDL_GameControllerOpen(int joystick_index);
        
        [DllImport("SDL2", CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_GameControllerClose(SDL_GameController gamecontroller);

        [DllImport("SDL2", CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_GameControllerAddMappingsFromRW(IntPtr rw);

        [DllImport("SDL2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr SDL_RWFromFile(string fileName, string mode);

        [DllImport("SDL2", CallingConvention = CallingConvention.Cdecl)]
        private static extern SDL_JoystickGUID SDL_JoystickGetDeviceGUID(int index);

        #endregion

        internal void Init()
        {
            _connectedGamepads = new Dictionary<SDL_JoystickGUID, Gamepad>(new SDLJoystickGuidComparer());

            SDL_Init(SDL_INIT_JOYSTICK);
            var result = SDL_GameControllerAddMappingsFromRW(SDL_RWFromFile("Input/GameControllerDb.txt", "r"));
            if (result < 0)
            {
                throw new Exception("Could not load Controller Mappings");
            }
        }

        internal void Poll()
        {
            var joyStickCount = SDL_NumJoysticks();
            SDL_GameController gameController;

            for (var i = 0; i < joyStickCount; i++)
            {
                if (!SDL_IsGameController(i)) return;
                var guid = SDL_JoystickGetDeviceGUID(i);

                gameController = SDL_GameControllerOpen(i);

                if (!_connectedGamepads.ContainsKey(guid))
                {
                    _connectedGamepads.Add(guid, new Gamepad());
                }

                
                
                SDL_GameControllerClose(gameController);
            }

            Console.WriteLine($"[{string.Join(',', _connectedGamepads.Select(x => x.ToString()))}]");
        }

    }
}
