using KanMach.Core;
using KanMach.Core.Interfaces;
using KanMach.Veldrid;
using KanMach.Veldrid.Util;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanMach.Sample
{
    public class Startup : IStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IVeldridService, VeldridService>();
        }

        public void Configure(KanGameEngine engine)
        {
            
        }
    }
}
