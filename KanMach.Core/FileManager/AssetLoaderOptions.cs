using Microsoft.Extensions.DependencyInjection;
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

        public IServiceCollection Services { get; set; }

        public AssetLoaderOptions(IServiceCollection serviceCollection)
        {
            Services = serviceCollection;
        }

        public void AddProcessor<T>() where T : class
        {
            Services.AddScoped<T>();
        }

    }
}
