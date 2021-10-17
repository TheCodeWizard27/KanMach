﻿using KanMach.Core;
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
        static void Main(string[] args)
        {
            var world = new EcsWorld();
            var rnd = new Random();
            
            {
                var entity = world.NewEntity();
                var test = entity.Get<TestStruct>();
                test.Counter = 0;
            }

            for (var i = 0; i < 10000; i++)
            {
                var entity = world.NewEntity();
                ref var transform = ref entity.Get<Transform>();
                transform.Pos = new Vector3(rnd.Next(-100, 100), rnd.Next(-100, 100), rnd.Next(-100, 100));
                transform.OnFloor = rnd.Next(0, 2) == 1;
            }

            var view = world.View<GameObjectView>();
            foreach(var id in view)
            {
                var transform = view.GetTransform(id);
                Console.WriteLine($"{{ x: {transform.Pos.X} \t\t| y: {transform.Pos.Y} \t\t| z: {transform.Pos.Z} }} \t\t[ Grounded: {transform.OnFloor} \t]");
            }
          
            var vs = new VeldridService();
          
            Console.ReadLine();
          
        }
    }
}
