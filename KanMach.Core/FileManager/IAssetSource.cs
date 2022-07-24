using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanMach.Core.FileManager
{
    public interface IAssetSource
    {

        Stream GetStream(string path);

    }
}
