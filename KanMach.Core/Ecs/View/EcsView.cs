using KanMach.Core.Ecs.View;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanMach.Core.Ecs.View
{

    // TODO Add on "OnAdd" and "OnRemove" functions probably rename to "Add" and "Remove" because they are not events.
    public abstract class EcsView
    {

        private ViewOperation[] _delayedOperations;

        protected EcsWorld World;

        protected Entity[] Entities;
        protected readonly Dictionary<int, int> EntitiesMap;

        protected int[] IncludedTypeIndices;
        protected int[] ExcludedTypeIndices;

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
        private int[] _get;
        private readonly ComponentPool<Inc> _pool;
        private Inc[] _incComponents;

        public ref Inc Get(in int id) => ref _incComponents[_get[id]];

        public EcsView(EcsWorld world) : base(world)
        {
            IncludedTypeIndices = new[] { ComponentType<Inc>.TypeId };
            IncludedTypes = new[] { typeof(Inc) };

            _pool = world.GetPool<Inc>();
            _incComponents = _pool.Components;
            _get = new int[world.Config.ViewEntityCacheSize];
        }

        public class Exclude<Exc> : EcsView<Inc>
            where Exc : struct
        {

            public Exclude(EcsWorld world) : base(world)
            {
                ExcludedTypeIndices = new[] { ComponentType<Exc>.TypeId };
                ExcludedTypes = new[] { typeof(Exc) };
            }

        }
        public class Exclude<Exc1, Exc2> : Exclude<Exc1>
            where Exc1 : struct
            where Exc2 : struct
        {

            public Exclude(EcsWorld world) : base(world)
            {
                ExcludedTypeIndices = new[] { 
                    ComponentType<Exc1>.TypeId,
                    ComponentType<Exc2>.TypeId
                };
                ExcludedTypes = new[] { 
                    typeof(Exc1),
                    typeof(Exc2)
                };
            }

        }
    }
    public class EcsView<Inc1, Inc2> : EcsView<Inc1>
        where Inc1 : struct
        where Inc2 : struct
    {
        private int[] _get1;
        private int[] _get2;

        private readonly ComponentPool<Inc1> _pool1;
        private readonly ComponentPool<Inc2> _pool2;

        private Inc1[] _incComponents1;
        private Inc2[] _incComponents2;

        public ref Inc1 Get1(in int id) => ref _incComponents1[_get1[id]];
        public ref Inc2 Get2(in int id) => ref _incComponents2[_get2[id]];

        public EcsView(EcsWorld world) : base(world)
        {
            IncludedTypeIndices = new[] { 
                ComponentType<Inc1>.TypeId,
                ComponentType<Inc2>.TypeId
            };
            IncludedTypes = new[] { 
                typeof(Inc1),
                typeof(Inc2)
            };

            _pool1 = world.GetPool<Inc1>();
            _pool2 = world.GetPool<Inc2>();
            _incComponents1 = _pool1.Components;
            _incComponents2 = _pool2.Components;
            _get1 = new int[world.Config.ViewEntityCacheSize];
            _get2 = new int[world.Config.ViewEntityCacheSize];
        }

        public class Exclude<Exc> : EcsView<Inc1, Inc2>
            where Exc : struct
        {
            public Exclude(EcsWorld world) : base(world)
            {
                ExcludedTypeIndices = new[] { ComponentType<Exc>.TypeId };
                ExcludedTypes = new[] { typeof(Exc) };
            }
        }
        public class Exclude<Exc1, Exc2> : Exclude<Exc1>
            where Exc1 : struct
            where Exc2 : struct
        {
            public Exclude(EcsWorld world) : base(world)
            {
                ExcludedTypeIndices = new[] {
                    ComponentType<Exc1>.TypeId,
                    ComponentType<Exc2>.TypeId
                };
                ExcludedTypes = new[] {
                    typeof(Exc1),
                    typeof(Exc2)
                };
            }
        }
    }
    public class EcsView<Inc1, Inc2, Inc3> : EcsView<Inc1, Inc2>
        where Inc1 : struct
        where Inc2 : struct
        where Inc3 : struct
    {
        private int[] _get1;
        private int[] _get2;
        private int[] _get3;

        private readonly ComponentPool<Inc1> _pool1;
        private readonly ComponentPool<Inc2> _pool2;
        private readonly ComponentPool<Inc3> _pool3;

        private Inc1[] _incComponents1;
        private Inc2[] _incComponents2;
        private Inc3[] _incComponents3;

        public ref Inc1 Get1(in int id) => ref _incComponents1[_get1[id]];
        public ref Inc2 Get2(in int id) => ref _incComponents2[_get2[id]];

        public EcsView(EcsWorld world) : base(world)
        {
            IncludedTypeIndices = new[] {
                ComponentType<Inc1>.TypeId,
                ComponentType<Inc2>.TypeId,
                ComponentType<Inc3>.TypeId
            };
            IncludedTypes = new[] {
                typeof(Inc1),
                typeof(Inc2),
                typeof(Inc3)
            };

            _pool1 = world.GetPool<Inc1>();
            _pool2 = world.GetPool<Inc2>();
            _pool3 = world.GetPool<Inc3>();
            _incComponents1 = _pool1.Components;
            _incComponents2 = _pool2.Components;
            _incComponents3 = _pool3.Components;
            _get1 = new int[world.Config.ViewEntityCacheSize];
            _get2 = new int[world.Config.ViewEntityCacheSize];
            _get3 = new int[world.Config.ViewEntityCacheSize];
        }

        public class Exclude<Exc> : EcsView<Inc1, Inc2, Inc3>
            where Exc : struct
        {
            public Exclude(EcsWorld world) : base(world)
            {
                ExcludedTypeIndices = new[] { ComponentType<Exc>.TypeId };
                ExcludedTypes = new[] { typeof(Exc) };
            }
        }
        public class Exclude<Exc1, Exc2> : Exclude<Exc1>
            where Exc1 : struct
            where Exc2 : struct
        {
            public Exclude(EcsWorld world) : base(world)
            {
                ExcludedTypeIndices = new[] {
                    ComponentType<Exc1>.TypeId,
                    ComponentType<Exc2>.TypeId
                };
                ExcludedTypes = new[] {
                    typeof(Exc1),
                    typeof(Exc2)
                };
            }
        }
    }

}
