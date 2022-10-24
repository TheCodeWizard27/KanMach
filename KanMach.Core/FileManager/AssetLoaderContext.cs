using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanMach.Core.FileManager
{
    public class AssetLoaderContext
    {

        private IAssetLoader _assetLoader;

        public string Path { get; set; }

        internal AssetLoaderContext(string path, IAssetLoader assetLoader)
        {
            Path = path;
            _assetLoader = assetLoader;
        }

        public AssetType Load<ProcessorType, AssetType>(string path) where ProcessorType : AssetProcessor<AssetType>
            => _assetLoader.Load<ProcessorType, AssetType>(path);
        public Stream Load(string path) => _assetLoader.Load(path);

    }
}
