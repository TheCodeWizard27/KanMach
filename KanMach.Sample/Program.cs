using KanMach.Core;
using KanMach.Core.Ecs;
using KanMach.Core.Ecs.Extensions;
using KanMach.Core.Ecs.View;
using KanMach.Core.Structures;
using KanMach.Core.Structures.Collision;
using KanMach.Veldrid;
using System;
using System.Numerics;

namespace KanMach.Sample
{

    class Program
    {

        public class Test
        {
            public string Name { get; set; }

            public Test(string name)
            {
                Name = name;
            }
        }

        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("Started KanMach");

            var octree = new Octree<Test>(new BoundingBox(new Vector3(0,0,0), new Vector3(1,1,1)));
            octree.AddItem(new BoundingBox(new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0.6f, 0.6f, 0.6f)), new Test("First"));
            octree.AddItem(new BoundingBox(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(0.6f, 0.6f, 0.6f)), new Test("Second"));

            //var engineBuilder = KanGameEngineBuilder
            //    .CreateDefaultBuilder()
            //    .SetStartup<Startup>()
            //    .Build();

            ////engineBuilder.Run<InputSampleController>();
            ////engineBuilder.Run<EcsHierarchyController>();
            ////engineBuilder.Run<SystemSampleController>();
            ////engineBuilder.Run<EcsSampleController>();
            //engineBuilder.Run<GraphicsSampleController>();

            Console.WriteLine("Program exited successfully");
            Console.ReadLine();
        }

    }
}
