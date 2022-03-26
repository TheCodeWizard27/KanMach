using KanMach.Veldrid.Input.SDL_Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace KanMach.Veldrid.Input
{

    internal class ValueResolver
    {

        public string Event { get; set; }
        public Func<IntPtr, ResolvedValue> Resolver { get; set; }

    }

    internal struct ResolvedValue
    {
        // Normalized value for axes can be everything between -1.0 to 1.0
        public float AxisValue;
        // Normalized value for buttons can be everything between 0 to 1.0
        public float ButtonValue;

        public bool IsPressed() => ButtonValue >= GamepadMap.VALUE_SENSITIVITY;

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
        public const float VALUE_SENSITIVITY = 0.2f;

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
                    case "-" + GamepadEvent.LEFT_X:
                        state.Buttons |= value.AxisValue <= -VALUE_SENSITIVITY ? GamepadButton.LeftThumbstickLeft : 0;
                        state.Left.X += value.AxisValue;
                        break;
                    case "+" + GamepadEvent.LEFT_X:
                        state.Buttons |= value.AxisValue >= VALUE_SENSITIVITY ? GamepadButton.LeftThumbstickRight : 0;
                        state.Left.X += value.AxisValue;
                        break;
                    case "-" + GamepadEvent.LEFT_Y:
                        state.Buttons |= value.AxisValue <= -VALUE_SENSITIVITY ? GamepadButton.LeftThumbstickUp : 0;
                        state.Left.Y += value.AxisValue;
                        break;
                    case "+" + GamepadEvent.LEFT_Y:
                        state.Buttons |= value.AxisValue >= VALUE_SENSITIVITY ? GamepadButton.LeftThumbstickDown : 0;
                        state.Left.Y += value.AxisValue;
                        break;
                    case "-" + GamepadEvent.RIGHT_X:
                        state.Buttons |= value.AxisValue <= -VALUE_SENSITIVITY ? GamepadButton.RightThumbstickLeft : 0;
                        state.Right.X += value.AxisValue;
                        break;
                    case "+" + GamepadEvent.RIGHT_X:
                        state.Buttons |= value.AxisValue >= VALUE_SENSITIVITY ? GamepadButton.RightThumbstickRight : 0;
                        state.Right.X += value.AxisValue;
                        break;
                    case "-" + GamepadEvent.RIGHT_Y:
                        state.Buttons |= value.AxisValue <= -VALUE_SENSITIVITY ? GamepadButton.RightThumbstickUp : 0;
                        state.Right.Y += value.AxisValue;
                        break;
                    case "+" + GamepadEvent.RIGHT_Y:
                        state.Buttons |= value.AxisValue >= VALUE_SENSITIVITY ? GamepadButton.RightThumbstickDown : 0;
                        state.Right.Y += value.AxisValue;
                        break;
                    case GamepadEvent.LEFT_X:
                        state.Buttons |= value.AxisValue <= -VALUE_SENSITIVITY ? GamepadButton.LeftThumbstickLeft : 0;
                        state.Buttons |= value.AxisValue >= VALUE_SENSITIVITY ? GamepadButton.LeftThumbstickRight : 0;
                        state.Left.X = value.AxisValue;
                        break;
                    case GamepadEvent.LEFT_Y:
                        state.Buttons |= value.AxisValue <= -VALUE_SENSITIVITY ? GamepadButton.LeftThumbstickUp : 0;
                        state.Buttons |= value.AxisValue >= VALUE_SENSITIVITY ? GamepadButton.LeftThumbstickDown : 0;
                        state.Left.Y = value.AxisValue;
                        break;
                    case GamepadEvent.LEFT_STICK:
                        state.Buttons |= value.IsPressed() ? GamepadButton.LeftStick : 0;
                        break;
                    case GamepadEvent.RIGHT_X:
                        state.Buttons |= value.AxisValue <= -VALUE_SENSITIVITY ? GamepadButton.RightThumbstickLeft : 0;
                        state.Buttons |= value.AxisValue >= VALUE_SENSITIVITY ? GamepadButton.RightThumbstickRight : 0;
                        state.Right.X = value.AxisValue;
                        break;
                    case GamepadEvent.RIGHT_Y:
                        state.Buttons |= value.AxisValue <= -VALUE_SENSITIVITY ? GamepadButton.RightThumbstickUp : 0;
                        state.Buttons |= value.AxisValue >= VALUE_SENSITIVITY ? GamepadButton.RightThumbstickDown : 0;
                        state.Right.Y = value.AxisValue;
                        break;
                    case GamepadEvent.RIGHT_STICK:
                        state.Buttons |= value.IsPressed() ? GamepadButton.RightStick : 0;
                        break;
                    case GamepadEvent.DP_LEFT:
                        state.Buttons |= value.IsPressed() ? GamepadButton.DPadLeft : 0;
                        break;
                    case GamepadEvent.DP_UP:
                        state.Buttons |= value.IsPressed() ? GamepadButton.DPadUp : 0;
                        break;
                    case GamepadEvent.DP_RIGHT:
                        state.Buttons |= value.IsPressed() ? GamepadButton.DPadRight : 0;
                        break;
                    case GamepadEvent.DP_DOWN:
                        state.Buttons |= value.IsPressed() ? GamepadButton.DPadDown : 0;
                        break;
                    case GamepadEvent.LEFT_TRIGGER:
                        state.Buttons |= value.IsPressed() ? GamepadButton.LeftTrigger : 0;
                        state.LeftTrigger = value.ButtonValue;
                        break;
                    case GamepadEvent.LEFT_SHOULDER:
                        state.Buttons |= value.IsPressed() ? GamepadButton.LeftShoulder : 0;
                        break;
                    case GamepadEvent.RIGHT_TRIGGER:
                        state.Buttons |= value.IsPressed() ? GamepadButton.RightTrigger : 0;
                        state.RightTrigger = value.ButtonValue;
                        break;
                    case GamepadEvent.RIGHT_SHOULDER:
                        state.Buttons |= value.IsPressed() ? GamepadButton.RightShoulder : 0;
                        break;
                    case GamepadEvent.BACK:
                        state.Buttons |= value.IsPressed() ? GamepadButton.Back : 0;
                        break;
                    case GamepadEvent.START:
                        state.Buttons |= value.IsPressed() ? GamepadButton.Start : 0;
                        break;
                    case GamepadEvent.GUIDE:
                        state.Buttons |= value.IsPressed() ? GamepadButton.BigButton : 0;
                        break;
                    case GamepadEvent.A:
                        state.Buttons |= value.IsPressed() ? GamepadButton.A : 0;
                        break;
                    case GamepadEvent.B:
                        state.Buttons |= value.IsPressed() ? GamepadButton.B : 0;
                        break;
                    case GamepadEvent.X:
                        state.Buttons |= value.IsPressed() ? GamepadButton.X : 0;
                        break;
                    case GamepadEvent.Y:
                        state.Buttons |= value.IsPressed() ? GamepadButton.Y : 0;
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
                    case '-':
                    case '+':
                        var axisId = Convert.ToInt32(value.TrimStart('-', '+', AXIS).TrimEnd(INVERT_MODIFIER));
                        resolver.Resolver = (joystickGuid) =>
                            ResolveAxisRangeValue(joystickGuid, axisId, value.Contains('+'), value.Contains(INVERT_MODIFIER));
                        break;
                    case AXIS:
                        var axisId2 = Convert.ToInt32(value.TrimStart(AXIS).TrimEnd(INVERT_MODIFIER));
                        resolver.Resolver = (joyStickGuid) => ResolveAxisValue(joyStickGuid, axisId2, value.Contains(INVERT_MODIFIER));
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

        private ResolvedValue ResolveButtonValue(IntPtr joystickGuid, int buttonId)
        {
            var value = SDL_JoystickGetButton(joystickGuid, buttonId);

            return new ResolvedValue
            {
                ButtonValue = value
            };
        }
        private ResolvedValue ResolveAxisRangeValue(IntPtr joystickGuid, int axisId, bool usePositiveRange, bool invertValue)
        {
            var axisValue = (float) SDL_JoystickGetAxis(joystickGuid, axisId);

            if ((usePositiveRange && axisValue < 0) || (!usePositiveRange && axisValue > 0)) return new ResolvedValue
            {
                AxisValue = 0,
                ButtonValue = 0
            };

            return new ResolvedValue
            {
                // TODO Improve? Same hack as in ResolveAxisValueMethod
                AxisValue = axisValue >= 0 ? axisValue / AXIS_RANGE : axisValue / (AXIS_RANGE + 1),
                ButtonValue = axisValue >= 0 ? axisValue / AXIS_RANGE : Math.Abs(axisValue / (AXIS_RANGE + 1))
            };
        }
        private ResolvedValue ResolveAxisValue(IntPtr joystickGuid, int axisId, bool invertValue)
        {
            var axisValue = (float) SDL_JoystickGetAxis(joystickGuid, axisId);

            if (invertValue) axisValue *= -1;

            return new ResolvedValue
            {
                // TODO Improve? Hack solves negative space being 1 greater.
                AxisValue = axisValue >= 0 ? axisValue / AXIS_RANGE : axisValue / (AXIS_RANGE+1),
                ButtonValue = (axisValue + AXIS_RANGE+1) / (AXIS_RANGE*2+1)
            };
        }
        private ResolvedValue ResolveHatValue(IntPtr joystickGuid, int hatId, int expectedHatValue)
        {
            var hatValue = SDL_JoystickGetHat(joystickGuid, hatId);

            return new ResolvedValue
            {
                ButtonValue = hatValue == expectedHatValue ? 1 : 0,
                AxisValue = GetHatStateVector(hatValue)
            };
        }

        private float GetHatStateVector(int hatState)
        {
            switch(hatState)
            {
                case 1: return -1;
                case 2: return 1;
                case 4: return 1;
                case 8: return -1;
                default: return 0;
            }
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
