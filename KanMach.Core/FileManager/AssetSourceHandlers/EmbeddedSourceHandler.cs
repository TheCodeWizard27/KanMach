using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KanMach.Core.FileManager.AssetSourceHandlers
{
    public class EmbeddedSourceHandler : IAssetSource
    {
        public Stream GetStream(string path)
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
        }
    }
}
