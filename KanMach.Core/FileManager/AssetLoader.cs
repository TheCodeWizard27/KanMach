using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;

namespace KanMach.Core.FileManager
{
    public class AssetLoader
    {

        private readonly Dictionary<Type, AssetProcessor> _processors;

        public AssetLoader(AssetLoaderOptions options)
        {
            _processors = options.Processors;
        }

        internal T Load<T>(Stream stream, AssetLoaderContext context) {
            return (T) Load(stream, typeof(T), context);
        }

        internal object Load(Stream stream, Type type, AssetLoaderContext context)
        {
            _processors.TryGetValue(type, out var processor);

            if (processor == null)
            {
                throw new InvalidOperationException($"No asset process for type {type} configured.");
            }

            return processor.ProcessObject(stream, context);
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
            return (T) Load(path, typeof(T));
        }

        private object Load(string path, Type type)
        {
            using (var stream = _assetSource.GetStream(path))
            {
                var context = new AssetLoaderContext(path, (path, type) => Load(path, type));
                return _assetLoader.Load(stream, type, context);
            }
        }

    }

}
