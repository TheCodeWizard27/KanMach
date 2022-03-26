﻿using KanMach.Core;
using KanMach.Core.Ecs;
using KanMach.Core.Ecs.Extensions;
using KanMach.Core.Interfaces;
using KanMach.Veldrid;
using KanMach.Veldrid.Input;
using System;
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
            foreach (var id in view)
            {
                var transform = view.GetTransform(id);
                //Console.WriteLine($"{{ x: {transform.Pos.X} \t\t| y: {transform.Pos.Y} \t\t| z: {transform.Pos.Z} }} \t\t[ Grounded: {transform.OnFloor} \t]");
            }
            Console.WriteLine($"Looped through 10000 Entities in {DateTime.Now - time}");
        }

        public override void Update(TimeSpan delta)
        {
        }

        public override void Dispose()
        {
        }
    }
}
