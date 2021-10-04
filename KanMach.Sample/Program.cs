using KanMach.Core;
using KanMach.Core.Ecs;
using KanMach.Core.Ecs.Extensions;
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

        }
    }
}
