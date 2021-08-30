using KanMach.Core.Interfaces;
using Microsoft.Extensions.Logging;
using System;

namespace KanMach.Core
{
    public class KanGameEngine
    {
        private readonly ILogger _logger;

        public IKanContext Context { get; set; }

        public KanGameEngine(
            ILogger logger,
            IKanContext context
            )
        {
            _logger = logger;
            Context = context;
        }

        public void Run()
        {

        }

        public void Update()
        {

        }

        public void Draw()
        {

        }

    }
}
