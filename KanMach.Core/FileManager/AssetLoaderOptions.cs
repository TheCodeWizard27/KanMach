using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanMach.Core.FileManager
{
    public class AssetLoaderOptions
    {

        internal readonly Dictionary<Type, AssetProcessor> Processors = new Dictionary<Type, AssetProcessor>();
        internal readonly List<Type> Sources = new List<Type>();

        public AssetLoaderOptions AddProcessor<T>(AssetProcessor<T> processor)
        {
            Processors.Add(typeof(T), processor);
            return this;
        }

        public AssetLoaderOptions AddSource<T>() where T : IAssetSource
        {
            Sources.Add(typeof(T));
            return this;
        }

    }
}
