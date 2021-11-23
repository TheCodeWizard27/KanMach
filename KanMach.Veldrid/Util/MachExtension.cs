using KanMach.Core;
using KanMach.Veldrid.Util.Options;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
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

            veldridService.Init();

            return engine;
        }

        public static IServiceCollection UseVeldridFrontend(this IServiceCollection services, Func<MachOptions, MachOptions> configurator = null)
        {
            var veldridService = new VeldridService(configurator?.Invoke(new MachOptions()));
            services.AddSingleton<IVeldridService>(veldridService);

            return services;
        }

    }
}
