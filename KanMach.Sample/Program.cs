using KanMach.Core;
using KanMach.Core.Ecs;
using KanMach.Core.Ecs.Extensions;
using KanMach.Core.Ecs.View;
using KanMach.Veldrid;
using System;

namespace KanMach.Sample
{

    struct TestStruct
    {
        public int Counter;
    }

    class Program
    {
        static void Main(string[] args)
        {
            var world = new EcsWorld();

            var entity = world.NewEntity();
            ref var testStruct = ref entity.Get<TestStruct>();
            testStruct.Counter = 1;

            var hasStruct = entity.Has<TestStruct>();

            world.View<EcsView<TestStruct>.Exclude<TestStruct>>();

            var vs = new VeldridService();
        }
    }
}
