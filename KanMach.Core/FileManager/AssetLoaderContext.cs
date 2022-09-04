using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanMach.Core.FileManager
{
    public class AssetLoaderContext
    {

        private Func<string, Type, object> _loadRedirect;

        public string Path { get; set; }

        internal AssetLoaderContext(string path, Func<string, Type, object> loadRedirect)
        {
            Path = path;
            _loadRedirect = loadRedirect;
        }

        public T Load<T>(string path) => (T) _loadRedirect.Invoke(path, typeof(T));

    }
}
