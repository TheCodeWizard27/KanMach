using KanMach.Core.FileManager;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanMach.Veldrid.AssetProcessors.AssimpProcessor
{
    public static class AssimpProcessorExtension
    {

        public static void RegisterAssimpProcessors(this AssetLoaderOptions options)
        {
            options.Services.AddScoped<AssimpModelProcessor>();
            options.Services.AddScoped<AssimpSceneLoader>();
        }

    }
}
