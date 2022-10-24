using KanMach.Core.FileManager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veldrid.ImageSharp;

namespace KanMach.Veldrid.AssetProcessors.AssimpProcessor
{
    public class ImageSharpTextureProcessor : AssetProcessor<ImageSharpTexture>
    {
        public override ImageSharpTexture Process(Stream stream, AssetLoaderContext context)
        {
            return new ImageSharpTexture(stream, true, false);
        }
    }
}
