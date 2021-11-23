using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanMach.Core.Interfaces
{
    public interface IStartup
    {

        public void ConfigureServices(IServiceCollection services);
        public void Configure(KanGameEngine engine);

    }
}
