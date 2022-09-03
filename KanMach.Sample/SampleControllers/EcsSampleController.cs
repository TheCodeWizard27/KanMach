using KanMach.Core;
using KanMach.Core.Ecs;
using KanMach.Core.Ecs.Extensions;
using KanMach.Core.Ecs.View;
using KanMach.Core.Interfaces;
using KanMach.Veldrid;
using KanMach.Veldrid.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace KanMach.Sample
{

    public class EcsSampleController : KanGameController
    {

        public EcsSampleController()
        {
        }

        public override void Init()
        {
            var world = new EcsWorld();
            var rnd = new Random();

            var time = DateTime.Now;

            {
                var entity = world.NewEntity();
                var test = entity.Get<TestStruct>();
                test.Counter = 0;
            }

            for (var i = 0; i < 100000; i++)
            {
                var entity = world.NewEntity();
                ref var transform = ref entity.Get<Transform>();
                transform.Pos = new Vector3(rnd.Next(-100, 100), rnd.Next(-100, 100), rnd.Next(-100, 100));
                transform.OnFloor = rnd.Next(0, 2) == 1;
            }

            Console.WriteLine($"Loaded 10000 Entities in {DateTime.Now - time}");
            time = DateTime.Now;

            var view = world.View<GameObjectView>();
            foreach (ViewEntity<Transform> entity in view)
            {
                //Console.WriteLine($"{{ x: {entity.Component.Pos.X} \t\t| y: {entity.Component.Pos.Y} \t\t| z: {entity.Component.Pos.Z} }} \t\t[ Grounded: {entity.Component.OnFloor} \t]");
            }
            Console.WriteLine($"Looped through 10000 Entities in {DateTime.Now - time}");
        }

        public override void Update(FrameTime delta)
        {
        }

        public override void Dispose()
        {
        }

        internal struct TestStruct
        {
            public int Counter;
        }

        internal struct Transform
        {
            public Vector3 Pos;
            public bool OnFloor;
        }

        internal class GameObjectView : EcsView<Transform>
        {

            public GameObjectView(EcsWorld world) : base(world)
            {
            }
        }

    }
}
