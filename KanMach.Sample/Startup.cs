using KanMach.Core;
using KanMach.Core.FileManager;
using KanMach.Core.FileManager.AssetSourceHandlers;
using KanMach.Core.Interfaces;
using KanMach.Veldrid.AssetProcessors;
using KanMach.Veldrid.AssetProcessors.AssimpProcessor;
using KanMach.Veldrid.Util;
using Microsoft.Extensions.DependencyInjection;
using Veldrid;

namespace KanMach.Sample
{
    public class Startup : IStartup
    {

        public void ConfigureServices(IServiceCollection services)
        {

            services.UseVeldridFrontend(opt =>
            {
                opt.Backend = GraphicsBackend.OpenGL;
                opt.WindowOptions.WindowWidth = 1000;
                opt.WindowOptions.WindowHeight = 900;
                opt.UseGamepads();
            });
            services.UseAssetManager(opt =>
            {
                opt.RegisterAssimpProcessors();
            });

        }

        public void Configure(KanGameEngine engine)
        {

            engine.UseVeldrid();

        }

    }
}
