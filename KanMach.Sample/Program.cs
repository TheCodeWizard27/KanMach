using KanMach.Core;
using KanMach.Core.Ecs;
using KanMach.Core.Ecs.Extensions;
using KanMach.Core.Ecs.View;
using KanMach.Veldrid;
using System;
using System.Numerics;

namespace KanMach.Sample
{

    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("Started KanMach");

            var engineBuilder = KanGameEngineBuilder
                .CreateDefaultBuilder()
                .SetStartup<Startup>()
                .Build();

            //engineBuilder.Run<InputSampleController>();
            //engineBuilder.Run<EcsHierarchyController>();
            //engineBuilder.Run<SystemSampleController>();
            //engineBuilder.Run<EcsSampleController>();
            engineBuilder.Run<GraphicsSampleController>();

            Console.WriteLine("Program exited successfully");
            Console.ReadLine();
        }

    }
}
