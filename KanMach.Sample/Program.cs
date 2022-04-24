using KanMach.Core;
using KanMach.Core.Ecs;
using KanMach.Core.Ecs.Extensions;
using KanMach.Core.Ecs.View;
using KanMach.Veldrid;
using System;
using System.Numerics;

namespace KanMach.Sample
{

    public struct TestStruct
    {
        public int Counter;
    }

    public struct Transform
    {
        public Vector3 Pos;
        public bool OnFloor;
    }

    public class GameObjectView : EcsView<Transform>
    {

        public ref Transform GetTransform(in int id) => ref Get(id);

        public GameObjectView(EcsWorld world) : base(world)
        {
        }
    }

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
