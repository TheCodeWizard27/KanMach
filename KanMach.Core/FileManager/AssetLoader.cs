using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;

namespace KanMach.Core.FileManager
{
    public class AssetLoader
    {

        private readonly IServiceProvider _serviceProvider;

        public AssetLoader(IServiceProvider provider)
        {
            _serviceProvider = provider;
        }

        internal T Load<T>(Stream stream, AssetLoaderContext context) {
            return (T) Load(stream, typeof(T), context);
        }

        internal object Load(Stream stream, Type type, AssetLoaderContext context)
        {
            using var scope = _serviceProvider.CreateScope();

            var processor = (AssetProcessor) ActivatorUtilities.CreateInstance(scope.ServiceProvider, type);

            return processor.ProcessObject(stream, context);
        }

    }

    public class AssetLoader<AssetSource> : IAssetLoader where AssetSource : IAssetSource 
    {

        private AssetLoader _assetLoader;
        private AssetSource _assetSource;

        public AssetLoader(AssetLoader assetLoader, IServiceProvider serviceProvider)
        {
            _assetLoader = assetLoader;
            _assetSource = (AssetSource) ActivatorUtilities.CreateInstance(serviceProvider, typeof(AssetSource));
        }

        public AssetType Load<ProcessorType, AssetType>(string path) where ProcessorType : AssetProcessor<AssetType>
        {
            return (AssetType) Load(path, typeof(ProcessorType));
        }

        public Stream Load(string path)
        {
            return _assetSource.GetStream(path);
        }

        private object Load(string path, Type type)
        {
            using (var stream = _assetSource.GetStream(path))
            {
                var context = new AssetLoaderContext(path, this);
                return _assetLoader.Load(stream, type, context);
            }
        }

    }

}
