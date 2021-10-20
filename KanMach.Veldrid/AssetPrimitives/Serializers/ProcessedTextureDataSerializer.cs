using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veldrid;

namespace KanMach.Veldrid.AssetPrimitives.Serializers
{
    public class ProcessedTextureDataSerializer : BinaryAssetSerializer<ProcessedTexture>
    {
        public override ProcessedTexture ReadT(BinaryReader reader)
        {
            return new ProcessedTexture(
                reader.ReadEnum<PixelFormat>(),
                reader.ReadEnum<TextureType>(),
                reader.ReadUInt32(),
                reader.ReadUInt32(),
                reader.ReadUInt32(),
                reader.ReadUInt32(),
                reader.ReadUInt32(),
                reader.ReadByteArray());
        }

        public override void WriteT(BinaryWriter writer, ProcessedTexture ptd)
        {
            writer.WriteEnum(ptd.Format);
            writer.WriteEnum(ptd.Type);
            writer.WriteEnum(ptd.Width);
            writer.WriteEnum(ptd.Height);
            writer.WriteEnum(ptd.Depth);
            writer.WriteEnum(ptd.MipLevels);
            writer.WriteEnum(ptd.ArrayLayers);
            writer.WriteEnum(ptd.TextureData);
        }
    }
}
