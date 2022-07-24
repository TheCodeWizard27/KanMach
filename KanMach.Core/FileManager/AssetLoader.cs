using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanMach.Core.FileManager
{
    public class AssetLoader
    {

        private readonly Dictionary<Type, AssetProcessor> _processors;

        public AssetLoader(AssetLoaderOptions options)
        {
            _processors = options.Processors;
        }

        public T Load<T>(Stream stream, string path = "")
        {
            _processors.TryGetValue(typeof(T), out var processor);

            if(processor == null)
            {
                throw new InvalidOperationException($"No asset process for type {typeof(T)} configured.");
            }

            return (T) processor.ProcessObject(stream, path);
        }

    }

    public class AssetLoader<AssetSource> where AssetSource : IAssetSource
    {

        private AssetLoader _assetLoader;
        private AssetSource _assetSource;

        public AssetLoader(AssetLoader assetLoader, IServiceProvider serviceProvider)
        {
            _assetLoader = assetLoader;
            _assetSource = (AssetSource) ActivatorUtilities.CreateInstance(serviceProvider, typeof(AssetSource));
        }

        public T Load<T>(string path)
        {
            using (var stream = _assetSource.GetStream(path))
            {
                return _assetLoader.Load<T>(stream);
            }
        }

    }

}
