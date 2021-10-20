using KanMach.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace KanMach.Core
{
    public class KanContext : IKanContext
    {
        internal KanGameEngine _kanGameEngine;

        public IServiceProvider Provider { get; private set; }

        internal KanContext(IServiceProvider serviceProvider)
        {
            Provider = serviceProvider;
        }

        public void SwapController(KanGameController controller, bool keepOld = false)
        {
            _kanGameEngine.SwapGameControllers(controller, keepOld);
        }
    }
}
