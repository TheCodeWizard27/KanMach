using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanMach.Core.FileManager
{
    public interface IAssetLoader
    {

        AssetType Load<ProcessorType, AssetType>(string path) where ProcessorType : AssetProcessor<AssetType>;
        Stream Load(string path);

    }
}
