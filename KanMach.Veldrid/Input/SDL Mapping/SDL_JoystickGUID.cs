using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace KanMach.Veldrid.Input.SDL_Mapping
{

    [StructLayout(LayoutKind.Sequential, Size = 16)]
    public struct SDL_JoystickGUID
    {
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] data;

        public override string ToString()
        {
            return string.Join("", data);
        }

        public override bool Equals(object obj)
        {
            return obj is SDL_JoystickGUID gUID && Equals(gUID);
        }

        public bool Equals(SDL_JoystickGUID other)
        {
            return ToString() == other.ToString();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(SDL_JoystickGUID left, SDL_JoystickGUID right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SDL_JoystickGUID left, SDL_JoystickGUID right)
        {
            return !(left == right);
        }

    }

    public class SDLJoystickGuidComparer : IEqualityComparer<SDL_JoystickGUID>
    {
        public bool Equals(SDL_JoystickGUID x, SDL_JoystickGUID y)
        {
            return x == y;
        }

        public int GetHashCode([DisallowNull] SDL_JoystickGUID obj)
        {
            return new Guid(obj.data).GetHashCode();
        }
    }

}
