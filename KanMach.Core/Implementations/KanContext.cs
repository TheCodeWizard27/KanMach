using KanMach.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace KanMach.Core
{
    public class KanContext : IKanContext
    {

        public IServiceProvider Provider { get; private set; }

        internal KanContext(IServiceProvider serviceProvider)
        {
            Provider = serviceProvider;
        }

    }
}
