using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanMach.Core.FileManager
{
    public static class AssetLoaderExtension
    {

        public static IServiceCollection UseAssetManager(this IServiceCollection services, Action<AssetLoaderOptions> configurator = null)
        {
            var loaderOptions = new AssetLoaderOptions();
            configurator?.Invoke(loaderOptions);
            var assetLoader = new AssetLoader(loaderOptions);
            services.AddSingleton(assetLoader);

            services.AddScoped(typeof(AssetLoader<>));

            return services;
        }

    }
}
