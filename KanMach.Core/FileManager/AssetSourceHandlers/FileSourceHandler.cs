using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanMach.Core.FileManager
{
    public class FileSourceHandler : IAssetSource
    {
        public Stream GetStream(string path)
        {
            return File.OpenRead(path);
        }
    }
}
