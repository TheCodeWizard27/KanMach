using KanMach.Core;
using KanMach.Core.Components;
using KanMach.Core.Ecs;
using KanMach.Core.Ecs.Extensions;
using KanMach.Core.Interfaces;
using KanMach.Veldrid;
using KanMach.Veldrid.Input;
using System;
using System.Numerics;

namespace KanMach.Sample
{
    public class EcsHierarchyController : KanGameController
    {
        private Random _rnd = new Random();


        public EcsHierarchyController()
        {
        }

        public override void Init()
        {
            var world = new EcsWorld();

            var entity = world.NewEntity();
            GenLayer(world, entity);

            PrintHierarchy(entity);
        }

        public void GenLayer(EcsWorld world, Entity entity)
        {
            var generateKids = Convert.ToBoolean(_rnd.Next(0,2));

            if (!generateKids) return;

            var kids = _rnd.Next(1, 3);

            for(var i = 0; i < kids; i++)
            {
                var newEntity = world.NewEntity();
                entity.Attach(newEntity);
                GenLayer(world, newEntity);
            }
        }

        public void PrintHierarchy(Entity? entity, int layer = 0)
        {
            if (!entity.HasValue) return;

            var relation = entity.Value.Get<Relation>();
            Console.WriteLine($"{MultiplyString("\t", layer)}> [{entity.Value.Id}]");

            PrintHierarchy(relation.FirstChild, layer+1);

            PrintHierarchy(relation.Next, layer);

        }

        public string MultiplyString(string text, int multiply)
        {
            var newText = "";
            for(var i = 0; i < multiply; i++)
            {
                newText += text;
            }

            return newText;
        }

        public override void Update(FrameTime delta)
        {
        }

        public override void Dispose()
        {
        }
    }
}
