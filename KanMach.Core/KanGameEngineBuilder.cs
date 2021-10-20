using KanMach.Core.Interfaces;
using KanMach.Core.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanMach.Core
{
    public class KanGameEngineBuilder
    {
        protected Type StartupType { get; set; }
        protected ILogger Logger { get; set; }

        protected KanGameEngineBuilder()
        {
        }

        public static KanGameEngineBuilder CreateDefaultBuilder()
        {
            return new KanGameEngineBuilder();
        }

        public KanGameEngineBuilder ConfigureLogging(ILogger logger)
        {
            Logger = logger;
            return this;
        }

        public KanGameEngineBuilder SetStartup<T>() where T : IStartup
        {
            StartupType = typeof(T);
            return this;
        } 

        public KanGameEngine Build()
        {
            var serviceCollection = new ServiceCollection();

            if(StartupType != null)
            {
                var startup = (IStartup)Activator.CreateInstance(StartupType);
                startup.ConfigureServices(serviceCollection);
            }
            Logger ??= new KanLogger();

            var provider = serviceCollection.BuildServiceProvider();
            var context = new KanContext(provider.CreateScope().ServiceProvider);
            var gameEngine = new KanGameEngine(Logger, context);
            context._kanGameEngine = gameEngine;

            return gameEngine;
        }

    }
}
