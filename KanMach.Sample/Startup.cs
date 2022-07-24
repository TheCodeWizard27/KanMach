using KanMach.Core;
using KanMach.Core.FileManager;
using KanMach.Core.FileManager.AssetSourceHandlers;
using KanMach.Core.Interfaces;
using KanMach.Veldrid;
using KanMach.Veldrid.AssetProcessors;
using KanMach.Veldrid.Util;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanMach.Sample
{
    public class Startup : IStartup
    {

        public void ConfigureServices(IServiceCollection services)
        {

            services.UseVeldridFrontend(opt =>
            {
                opt.UseGamepads();
            });
            services.UseAssetManager(opt =>
            {
                opt.AddProcessor(new AssimpMeshProcessor());

                opt
                    .AddSource<FileSourceHandler>()
                    .AddSource<EmbeddedSourceHandler>();
            });

        }

        public void Configure(KanGameEngine engine)
        {

            engine.UseVeldrid();

        }

    }
}
