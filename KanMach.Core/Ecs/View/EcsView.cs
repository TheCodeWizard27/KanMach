using KanMach.Core.Ecs.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanMach.Core.Ecs.View
{
    public abstract class EcsView
    {

        private ViewOperation[] _delayedOperations;

        protected EcsWorld World;

        protected Entity[] Entities;
        protected readonly Dictionary<int, int> EntitiesMap;

        protected internal int[] IncludedTypeIndices;
        protected internal int[] ExcludedTypeIndices;

        public Type[] IncludedTypes;
        public Type[] ExcludedTypes;

        internal EcsView(EcsWorld world)
        {
            World = world;
            Entities = new Entity[world.Config.ViewEntityCacheSize];
            EntitiesMap = new Dictionary<int, int>(world.Config.ViewEntityCacheSize);
            _delayedOperations = new ViewOperation[world.Config.ViewEntityCacheSize];
        }

    }

    public class EcsView<Inc> : EcsView
        where Inc : struct
    {

        public EcsView(EcsWorld world) : base(world)
        {

        }

        public class Exclude<Exc> : EcsView<Inc>
        {
            public Exclude(EcsWorld world) : base(world)
            {

            }
        }
        public class Exclude<Exc1, Exc2> : EcsView<Inc>.Exclude<Exc1>
        {
            public Exclude(EcsWorld world) : base(world)
            {

            }
        }
    }
    public class EcsView<Inc1, Inc2> : EcsView<Inc1>
        where Inc1 : struct
        where Inc2 : struct
    {

        public EcsView(EcsWorld world) : base(world)
        {

        }

        public class Exclude<Exc> : EcsView<Inc1, Inc2>
        {
            public Exclude(EcsWorld world) : base(world)
            {

            }
        }
        public class Exclude<Exc1, Exc2> : EcsView<Inc1,Inc2>.Exclude<Exc1>
        {
            public Exclude(EcsWorld world) : base(world)
            {

            }
        }
    }
    public class EcsView<Inc1, Inc2, Inc3> : EcsView<Inc1, Inc2>
        where Inc1 : struct
        where Inc2 : struct
        where Inc3 : struct
    {

        public EcsView(EcsWorld world) : base(world)
        {

        }

        public class Exclude<Exc> : EcsView<Inc1, Inc2, Inc3>
        {
            public Exclude(EcsWorld world) : base(world)
            {

            }
        }
        public class Exclude<Exc1, Exc2> : EcsView<Inc1, Inc2, Inc3>.Exclude<Exc1>
        {
            public Exclude(EcsWorld world) : base(world)
            {

            }
        }
    }

}
