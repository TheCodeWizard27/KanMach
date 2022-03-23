using KanMach.Veldrid.Input.SDL_Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace KanMach.Veldrid.Input
{

    public class ValueResolver
    {

        public string Event { get; set; }
        public Func<IntPtr, float> Resolver { get; set; }

    }

    public class GamepadMap
    {

        public static int BUTTON_OFFSET = 2;
        public static int PLATFORM_OFFSET = 3;

        public const char AXIS = 'a';
        public const char HAT = 'h';
        public const char BUTTON = 'b';
        public const char INVERT_MODIFIER = '~';

        public const int AXIS_RANGE = 32767;
        public const int AXIS_DEADZONE = 6553;

        public string Id { get; set; }
        public string Name { get; set; }
        public string Platform { get; set; }

        private bool _isLoaded = false;
        private string _mapConfiguration;
        private List<ValueResolver> _valueResolvers = new List<ValueResolver>();

        [DllImport("SDL2", CallingConvention = CallingConvention.Cdecl)]
        private static extern byte SDL_JoystickGetButton(IntPtr joystick, int buttonId);
        [DllImport("SDL2", CallingConvention = CallingConvention.Cdecl)]
        private static extern short SDL_JoystickGetAxis(IntPtr joystick, int axisId);
        [DllImport("SDL2", CallingConvention = CallingConvention.Cdecl)]
        private static extern byte SDL_JoystickGetHat(IntPtr joystick, int hatId);

        //
        // Example of mapping string
        //
        // BUTTON_OFFSET -------------------------------->                                              <- PLATFORM_OFFSET
        // <             ID               >,<    Name    >,<b1>,<b2>, . . .                            ,<   platform   >,
        // 03000000571d00002100000000000000,NES Controller,a:b0,b:b1,back:b2,leftx:a0,lefty:a1,start:b3,platform:Windows, 
        // 
        // 
        // Make up of mapping definition
        // 
        // <name>:<type><index>
        // a:b0
        //

        public GamepadMap(string mapConfiguration)
        {
            _mapConfiguration = mapConfiguration.Replace("\r", "");
            var values = _mapConfiguration.Split(",");

            Id = values[0];
            Name = values[1];
            Platform = values[values.Length - PLATFORM_OFFSET + 1].Split(":")[1];

            Load();
        }

        public GamepadState PollState(IntPtr handle)
        {
            var state = new GamepadState();

            _valueResolvers.ForEach((resolver) => {
                var value = resolver.Resolver(handle);

                switch(resolver.Event)
                {
                    case GamepadEvent.LEFT_X:
                        state.Buttons |= value <= -AXIS_DEADZONE ? GamepadButton.LeftThumbstickLeft : 0;
                        state.Buttons |= value >= AXIS_DEADZONE ? GamepadButton.LeftThumbstickRight : 0;
                        state.Left.X = value;
                        break;
                    case GamepadEvent.LEFT_Y:
                        state.Buttons |= value <= -AXIS_DEADZONE ? GamepadButton.LeftThumbstickUp : 0;
                        state.Buttons |= value >= AXIS_DEADZONE ? GamepadButton.LeftThumbstickDown : 0;
                        state.Left.Y = value;
                        break;
                    case GamepadEvent.LEFT_STICK:
                        state.Buttons |= value == 1 ? GamepadButton.LeftStick : 0;
                        break;
                    case GamepadEvent.RIGHT_X:
                        state.Buttons |= value <= -AXIS_DEADZONE ? GamepadButton.RightThumbstickLeft : 0;
                        state.Buttons |= value >= AXIS_DEADZONE ? GamepadButton.RightThumbstickRight : 0;
                        state.Right.X = value;
                        break;
                    case GamepadEvent.RIGHT_Y:
                        state.Buttons |= value <= -AXIS_DEADZONE ? GamepadButton.RightThumbstickUp : 0;
                        state.Buttons |= value >= AXIS_DEADZONE ? GamepadButton.RightThumbstickDown : 0;
                        state.Right.Y = value;
                        break;
                    case GamepadEvent.RIGHT_STICK:
                        state.Buttons |= value == 1 ? GamepadButton.RightStick : 0;
                        break;
                    case GamepadEvent.DP_LEFT:
                        state.Buttons |= value == 1 ? GamepadButton.DPadLeft : 0;
                        break;
                    case GamepadEvent.DP_UP:
                        state.Buttons |= value == 1 ? GamepadButton.DPadUp : 0;
                        break;
                    case GamepadEvent.DP_RIGHT:
                        state.Buttons |= value == 1 ? GamepadButton.DPadRight : 0;
                        break;
                    case GamepadEvent.DP_DOWN:
                        state.Buttons |= value == 1 ? GamepadButton.DPadDown : 0;
                        break;
                    case GamepadEvent.LEFT_TRIGGER:
                        state.Buttons |= value >= -AXIS_RANGE ? GamepadButton.LeftTrigger : 0;
                        state.LeftTrigger = value;
                        break;
                    case GamepadEvent.LEFT_SHOULDER:
                        state.Buttons |= value == 1 ? GamepadButton.LeftShoulder : 0;
                        break;
                    case GamepadEvent.RIGHT_TRIGGER:
                        state.Buttons |= value >= -AXIS_RANGE ? GamepadButton.LeftTrigger : 0;
                        state.RightTrigger = value;
                        break;
                    case GamepadEvent.RIGHT_SHOULDER:
                        state.Buttons |= value == 1 ? GamepadButton.RightShoulder : 0;
                        break;
                    case GamepadEvent.BACK:
                        state.Buttons |= value == 1 ? GamepadButton.Back : 0;
                        break;
                    case GamepadEvent.START:
                        state.Buttons |= value == 1 ? GamepadButton.Start : 0;
                        break;
                    case GamepadEvent.GUIDE:
                        state.Buttons |= value == 1 ? GamepadButton.BigButton : 0;
                        break;
                    case GamepadEvent.A:
                        state.Buttons |= value == 1 ? GamepadButton.A : 0;
                        break;
                    case GamepadEvent.B:
                        state.Buttons |= value == 1 ? GamepadButton.B : 0;
                        break;
                    case GamepadEvent.X:
                        state.Buttons |= value == 1 ? GamepadButton.X : 0;
                        break;
                    case GamepadEvent.Y:
                        state.Buttons |= value == 1 ? GamepadButton.Y : 0;
                        break;
                }
            });

            return state;    

        }

        public static IEnumerable<GamepadMap> CreateMapping(string controllerDb)
        {
            return controllerDb
                .Split("\r\n")
                .Select(PrepareLine)
                .Where( line => !string.IsNullOrEmpty(line))
                .Select( mapping => new GamepadMap(mapping))
                .ToList();
        }

        public void Load()
        {
            var mapConfiguration = _mapConfiguration.Split(",");

            for (var i = BUTTON_OFFSET; i <= mapConfiguration.Length - PLATFORM_OFFSET; i++)
            {
                var mapping = mapConfiguration[i].Split(":");

                var identifier = mapping[0];
                var value = mapping[1];

                var resolver = new ValueResolver();
                resolver.Event = identifier;

                switch(value.ElementAt(0))
                {
                    case BUTTON:
                        var btnId = Convert.ToInt32(value.TrimStart(BUTTON));
                        resolver.Resolver = (joyStickGuid) => ResolveButtonValue(joyStickGuid, btnId);
                        break;
                    case AXIS:
                        var axisId = Convert.ToInt32(value.TrimStart(AXIS).TrimEnd(INVERT_MODIFIER));
                        resolver.Resolver = (joyStickGuid) => ResolveAxisValue(joyStickGuid, axisId, value.Contains(INVERT_MODIFIER));
                        break;
                    case HAT:
                        var hatInfo = value.TrimStart(HAT).Split('.');
                        var hatId = Convert.ToInt32(hatInfo[0]);
                        var hatValue = Convert.ToInt32(hatInfo[1]);
                        resolver.Resolver = (joyStickGuid) => ResolveHatValue(joyStickGuid, hatId, hatValue);
                        break;
                }

                _valueResolvers.Add(resolver);

            }

            _isLoaded = true;
        }

        private float ResolveButtonValue(IntPtr joystickGuid, int buttonId)
        {
            return SDL_JoystickGetButton(joystickGuid, buttonId);
        }
        private float ResolveAxisValue(IntPtr joystickGuid, int axisId, bool invertValue)
        {
            var axisValue = SDL_JoystickGetAxis(joystickGuid, axisId);

            return invertValue ? axisValue * -1 : axisValue;
        }
        private float ResolveHatValue(IntPtr joystickGuid, int hatId, int expectedHatValue)
        {
            return SDL_JoystickGetHat(joystickGuid, hatId) == expectedHatValue ? 1 : 0;
        }

        private static string PrepareLine(string line)
        {
            line = line.Trim();

            // Trim comments.
            if (line.Contains("#"))
                return line.Substring(0, line.IndexOf("#"));

            return line;
        }


    }
}
