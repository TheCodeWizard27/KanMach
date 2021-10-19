using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace KanMach.Veldrid.AssetPrimitives
{
    public static class BinaryExtensions
    {
        public static unsafe T ReadEnum<T>(this BinaryReader reader)
        {
            int i32 = reader.ReadInt32();
            return Unsafe.Read<T>(&i32);
        }

        public static void WriteEnum<T>(this BinaryWriter writer, T value)
        {
            int i32 = Convert.ToInt32(value);
            writer.Write(i32);
        }

        public static byte[] ReadByteArray(this BinaryReader reader)
        {
            int byteCounter = reader.ReadInt32();
            return reader.ReadBytes(byteCounter);
        }
    }
}
