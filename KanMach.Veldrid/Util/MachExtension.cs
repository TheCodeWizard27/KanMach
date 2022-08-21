using KanMach.Core;
using KanMach.Veldrid.Input;
using KanMach.Veldrid.Util.Options;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KanMach.Veldrid.Util
{
    public static class MachExtension
    {

        public static KanGameEngine UseVeldrid(this KanGameEngine engine)
        {
            var veldridService = engine.Context.Provider.GetService<IVeldridService>();
            veldridService.OnClose += engine.Exit;
            engine.OnExit += veldridService.Close;
            engine.OnUpdate += veldridService.Update;

            veldridService.Init();

            var inputManager = engine.Context.Provider.GetService<Sdl2InputManager>();
            inputManager.StartListening();

            return engine;
        }

        public static IServiceCollection UseVeldridFrontend(this IServiceCollection services, Action<MachOptions> configurator = null)
        {
            var machOptions = new MachOptions();
            configurator?.Invoke(machOptions);

            var veldridService = new VeldridService(machOptions);
            services.AddSingleton<IVeldridService>(veldridService);

            var sdl2InputManager = new Sdl2InputManager(machOptions.Sdl2InputManagerOptions);
            services.AddSingleton(sdl2InputManager);

            return services;
        }

        public static string GetEmbeddedRessource(this Assembly assembly, string ressource)
        {
            using (var ressourceStream = assembly.GetManifestResourceStream(ressource))
            {
                using (var reader = new StreamReader(ressourceStream))
                    return reader.ReadToEnd();
            }
        }

    }
}
