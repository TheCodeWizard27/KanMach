using KanMach.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace KanMach.Core
{
    public class KanContext : IKanContext
    {
        public KanGameEngine KanGameEngine { get; internal set; }
        public IServiceProvider Provider { get; private set; }

        internal KanContext(IServiceProvider serviceProvider)
        {
            Provider = serviceProvider;
        }

        public IKanContext CreateNewScope()
        {
            return new KanContext(Provider.CreateScope().ServiceProvider);
        }

        public void SwapController(KanGameController controller, bool keepOld = false)
        {
            KanGameEngine.SwapGameControllers(controller, keepOld);
        }
    }
}
