using KanMach.Core.Ecs.View;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace KanMach.Core.Ecs.View
{

    public abstract class EcsView
    {

        private bool _lock;
        private GrowList<ViewOperation> _delayedOperations;

        protected EcsWorld World;

        protected Entity[] Entities;
        protected int EntitiesIndex;

        protected readonly Dictionary<int, int> EntitiesMap;

        internal protected int[] IncludedTypeIndices;
        internal protected int[] ExcludedTypeIndices;

        public Type[] IncludedTypes;
        public Type[] ExcludedTypes;

        internal EcsView(EcsWorld world)
        {
            World = world;
            Entities = new Entity[world.Config.ViewEntityCacheSize];
            EntitiesMap = new Dictionary<int, int>(world.Config.ViewEntityCacheSize);
            _delayedOperations = new GrowList<ViewOperation>(world.Config.ViewEntityCacheSize);
        }

        /// <summary>
        /// Checks if Entity is compatible
        /// </summary>
        /// <param name="entityData"></param>
        /// <param name="typeIndex">
        ///     Pass typeIndex of Componetn for optimized compatibility checks.
        ///     Type index is -/+ depending on if the Component got added or removed.
        /// </param>
        /// <returns></returns>
        internal bool IsCompatible(in EntityData entityData, int modifiedType = 0)
        {
            var includeIndex = IncludedTypeIndices.Length - 1;
            for(;includeIndex >= 0; includeIndex--)
            {
                var includedType = IncludedTypeIndices[includeIndex];

                var componentIndex = entityData.ComponentIndex - 1;
                for (; componentIndex >= 0; componentIndex--) 
                {
                    var type = entityData.ComponentTypes[componentIndex];
                    if(type == -modifiedType)
                    {
                        continue;
                    }
                    if(type == modifiedType || type  == includedType)
                    {
                        break;
                    }
                }

                if(componentIndex == -1)
                {
                    // Component not found.
                    break;
                }
            }

            if(includeIndex != -1)
            {
                // Not all included Components where found.
                return false;
            }

            if(ExcludedTypeIndices != null)
            {
                for(var i = 0; i < ExcludedTypeIndices.Length; i++)
                {
                    var excludedType = ExcludedTypeIndices[i];
                    for(var typeIndex = 0; typeIndex < entityData.ComponentIndex; typeIndex++)
                    {
                        var type = entityData.ComponentTypes[typeIndex];
                        if (type == -modifiedType)
                        {
                            continue;
                        }
                        if (type == modifiedType || type == excludedType)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        public abstract void AddEntity(in Entity entity);
        public abstract void RemoveEntity(in Entity entity);

        protected bool TryBufferingOperationIfLocked(bool isAddOperation, Entity entity)
        {
            if (!_lock) return false;

            _delayedOperations.Add();
            ref var operation = ref _delayedOperations.Items[_delayedOperations.Index];
            operation.IsAddAction = isAddOperation;
            operation.Entity = entity;

            return true;
        }

        protected void Lock()
        {
            _lock = true;
        }
        protected void Unlock()
        {
            _lock = false;
            while(_delayedOperations.Index > 0)
            {
                var operation = _delayedOperations.Take();
                if(operation.IsAddAction)
                {
                    AddEntity(operation.Entity);
                } else
                {
                    RemoveEntity(operation.Entity);
                }
            }
        }

        public abstract class BaseViewEnumerator<T, ViewType> : IEnumerator<T>
            where ViewType : EcsView
        {
            protected readonly ViewType _view;
            protected readonly private int _count;
            protected int _index;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal BaseViewEnumerator(ViewType view)
            {
                _view = view;
                _count = _view.EntitiesIndex;
                _index = -1;
                _view.Lock();
            }

            public abstract T Current { get; }

            object IEnumerator.Current => Current;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Dispose() => _view.Unlock();

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext() => ++_index < _count;

            public void Reset()
            {
                _index = -1;
            }
        }

    }

    public class EcsView<Inc> : EcsView, IEnumerable<ViewEntity<Inc>>
        where Inc : struct
    {
        internal int[] _get1;
        internal ComponentPool<Inc> _pool1;
        internal Inc[] _incComponents1;

        public ref Inc Get1(in int id) => ref _incComponents1[_get1[id]];

        public EcsView(EcsWorld world) : base(world)
        {
            IncludedTypeIndices = new[] { ComponentType<Inc>.TypeId };
            IncludedTypes = new[] { typeof(Inc) };

            _pool1 = world.GetPool<Inc>();
            _pool1.OnResize += _pool_OnResize;
            _incComponents1 = _pool1.Components;
            _get1 = new int[world.Config.ViewEntityCacheSize];
        }

        public override void AddEntity(in Entity entity)
        {
            if (TryBufferingOperationIfLocked(true, entity)) return;
            if (Entities.Length == EntitiesIndex)
            {
                var newLength = EntitiesIndex * 2;
                Array.Resize(ref Entities, newLength);
                Array.Resize(ref _get1, newLength);
            }

            ref var entityData = ref entity.World.GetEntityData(entity);
            for (int i = 0, left = 1; left > 0 && i < entityData.ComponentIndex; i++) {
                if (entityData.ComponentTypes[i] == ComponentType<Inc>.TypeId)
                {
                    _get1[EntitiesIndex] = entityData.ComponentIds[i];
                    left--;
                }
            }

            EntitiesMap[entity.Id] = EntitiesIndex;
            Entities[EntitiesIndex++] = entity;
        }

        public override void RemoveEntity(in Entity entity)
        {
            if (TryBufferingOperationIfLocked(false, entity)) return;
            var id = EntitiesMap[entity.Id];
            EntitiesMap.Remove(entity.Id);
            EntitiesIndex--;

            //Replace removed entity with newest one.
            if (id < EntitiesIndex)
            {
                Entities[id] = Entities[EntitiesIndex];
                EntitiesMap[Entities[id].Id] = id;

                _get1[id] = _get1[EntitiesIndex];
            }
        }

        private void _pool_OnResize()
        {
            _incComponents1 = _pool1.Components;
        }

        IEnumerator<ViewEntity<Inc>> IEnumerable<ViewEntity<Inc>>.GetEnumerator()
        {
            return new Enumerator(this);
        }

        public IEnumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        public class Enumerator : BaseViewEnumerator<ViewEntity<Inc>, EcsView<Inc>>
        {
            public Enumerator(EcsView<Inc> view) : base(view)
            {
            }

            public override ViewEntity<Inc> Current { get
                {
                    var viewEntity = new ViewEntity<Inc>();
                    viewEntity._view = _view;
                    viewEntity.Entity = _index;

                    return viewEntity;
                } 
            }
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
    public class EcsView<Inc1, Inc2> : EcsView<Inc1>, IEnumerable<ViewEntity<Inc1, Inc2>>
        where Inc1 : struct
        where Inc2 : struct
    {
        internal int[] _get2;
        internal ComponentPool<Inc2> _pool2;
        internal Inc2[] _incComponents2;

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
            _pool1.OnResize += _pool_OnResize;
            _pool2 = world.GetPool<Inc2>();
            _pool2.OnResize += _pool_OnResize;
            _incComponents1 = _pool1.Components;
            _incComponents2 = _pool2.Components;
            _get1 = new int[world.Config.ViewEntityCacheSize];
            _get2 = new int[world.Config.ViewEntityCacheSize];
        }

        public override void AddEntity(in Entity entity)
        {
            if (TryBufferingOperationIfLocked(true, entity)) return;
            if (Entities.Length == EntitiesIndex)
            {
                var newLength = EntitiesIndex * 2;
                Array.Resize(ref Entities, newLength);
                Array.Resize(ref _get1, newLength);
                Array.Resize(ref _get2, newLength);
            }

            ref var entityData = ref entity.World.GetEntityData(entity);
            for (int i = 0, left = 2; left > 0 && i < entityData.ComponentIndex; i++)
            {
                if (entityData.ComponentTypes[i] == ComponentType<Inc1>.TypeId)
                {
                    _get1[EntitiesIndex] = entityData.ComponentIds[i];
                    left--;
                    continue;
                }
                if (entityData.ComponentTypes[i] == ComponentType<Inc2>.TypeId)
                {
                    _get2[EntitiesIndex] = entityData.ComponentIds[i];
                    left--;
                }
            }

            EntitiesMap[entity.Id] = EntitiesIndex;
            Entities[EntitiesIndex++] = entity;
        }

        public override void RemoveEntity(in Entity entity)
        {
            if (TryBufferingOperationIfLocked(false, entity)) return;
            var id = EntitiesMap[entity.Id];
            EntitiesMap.Remove(entity.Id);
            EntitiesIndex--;

            //Replace removed entity with newest one.
            if (id < EntitiesIndex)
            {
                Entities[id] = Entities[EntitiesIndex];
                EntitiesMap[Entities[id].Id] = id;

                _get1[id] = _get1[EntitiesIndex];
                _get2[id] = _get2[EntitiesIndex];
            }
        }

        private void _pool_OnResize()
        {
            _incComponents1 = _pool1.Components;
            _incComponents2 = _pool2.Components;
        }
        IEnumerator<ViewEntity<Inc1, Inc2>> IEnumerable<ViewEntity<Inc1, Inc2>>.GetEnumerator()
        {
            return new Enumerator(this);
        }

        new public IEnumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        new public class Enumerator : BaseViewEnumerator<ViewEntity<Inc1, Inc2>, EcsView<Inc1, Inc2>>
        {
            public Enumerator(EcsView<Inc1, Inc2> view) : base(view)
            {
            }

            public override ViewEntity<Inc1, Inc2> Current
            {
                get
                {
                    var viewEntity = new ViewEntity<Inc1, Inc2>();
                    viewEntity._view = _view;
                    viewEntity.Entity = _index;

                    return viewEntity;
                }
            }
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

    public class EcsView<Inc1, Inc2, Inc3> : EcsView<Inc1, Inc2>, IEnumerable<ViewEntity<Inc1, Inc2, Inc3>>
        where Inc1 : struct
        where Inc2 : struct
        where Inc3 : struct
    {
        internal int[] _get3;
        internal ComponentPool<Inc3> _pool3;
        internal Inc3[] _incComponents3;

        public ref Inc3 Get3(in int id) => ref _incComponents3[_get3[id]];

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
            _pool1.OnResize += _pool_OnResize;
            _pool2 = world.GetPool<Inc2>();
            _pool2.OnResize += _pool_OnResize;
            _pool3 = world.GetPool<Inc3>();
            _pool3.OnResize += _pool_OnResize;
            _incComponents1 = _pool1.Components;
            _incComponents2 = _pool2.Components;
            _incComponents3 = _pool3.Components;
            _get1 = new int[world.Config.ViewEntityCacheSize];
            _get2 = new int[world.Config.ViewEntityCacheSize];
            _get3 = new int[world.Config.ViewEntityCacheSize];
        }

        public override void AddEntity(in Entity entity)
        {
            if (TryBufferingOperationIfLocked(true, entity)) return;
            if (Entities.Length == EntitiesIndex)
            {
                var newLength = EntitiesIndex * 2;
                Array.Resize(ref Entities, newLength);
                Array.Resize(ref _get1, newLength);
                Array.Resize(ref _get2, newLength);
                Array.Resize(ref _get3, newLength);
            }

            ref var entityData = ref entity.World.GetEntityData(entity);
            for (int i = 0, left = 3; left > 0 && i < entityData.ComponentIndex; i++)
            {
                if (entityData.ComponentTypes[i] == ComponentType<Inc1>.TypeId)
                {
                    _get1[EntitiesIndex] = entityData.ComponentIds[i];
                    left--;
                    continue;
                }
                if (entityData.ComponentTypes[i] == ComponentType<Inc2>.TypeId)
                {
                    _get2[EntitiesIndex] = entityData.ComponentIds[i];
                    left--;
                    continue;
                }
                if (entityData.ComponentTypes[i] == ComponentType<Inc3>.TypeId)
                {
                    _get3[EntitiesIndex] = entityData.ComponentIds[i];
                    left--;
                }
            }

            EntitiesMap[entity.Id] = EntitiesIndex;
            Entities[EntitiesIndex++] = entity;
        }

        public override void RemoveEntity(in Entity entity)
        {
            if (TryBufferingOperationIfLocked(false, entity)) return;
            var id = EntitiesMap[entity.Id];
            EntitiesMap.Remove(entity.Id);
            EntitiesIndex--;

            //Replace removed entity with newest one.
            if (id < EntitiesIndex)
            {
                Entities[id] = Entities[EntitiesIndex];
                EntitiesMap[Entities[id].Id] = id;

                _get1[id] = _get1[EntitiesIndex];
                _get2[id] = _get2[EntitiesIndex];
                _get3[id] = _get3[EntitiesIndex];
            }
        }

        private void _pool_OnResize()
        {
            _incComponents1 = _pool1.Components;
            _incComponents2 = _pool2.Components;
            _incComponents3 = _pool3.Components;
        }

        IEnumerator<ViewEntity<Inc1, Inc2, Inc3>> IEnumerable<ViewEntity<Inc1, Inc2, Inc3>>.GetEnumerator()
        {
            return new Enumerator(this);
        }

        new public IEnumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        new public class Enumerator : BaseViewEnumerator<ViewEntity<Inc1, Inc2, Inc3>, EcsView<Inc1, Inc2, Inc3>>
        {
            public Enumerator(EcsView<Inc1, Inc2, Inc3> view) : base(view)
            {
            }

            public override ViewEntity<Inc1, Inc2, Inc3> Current
            {
                get
                {
                    var viewEntity = new ViewEntity<Inc1, Inc2, Inc3>();
                    viewEntity._view = _view;
                    viewEntity.Entity = _index;

                    return viewEntity;
                }
            }
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

    public class EcsView<Inc1, Inc2, Inc3, Inc4> : EcsView<Inc1, Inc2, Inc3>, IEnumerable<ViewEntity<Inc1, Inc2, Inc3, Inc4>>
        where Inc1 : struct
        where Inc2 : struct
        where Inc3 : struct
        where Inc4 : struct
    {
        internal int[] _get4;
        internal ComponentPool<Inc4> _pool4;
        internal Inc4[] _incComponents4;
        public ref Inc4 Get4(in int id) => ref _incComponents4[_get4[id]];

        public EcsView(EcsWorld world) : base(world)
        {
            IncludedTypeIndices = new[] {
                ComponentType<Inc1>.TypeId,
                ComponentType<Inc2>.TypeId,
                ComponentType<Inc3>.TypeId,
                ComponentType<Inc4>.TypeId
            };
            IncludedTypes = new[] {
                typeof(Inc1),
                typeof(Inc2),
                typeof(Inc3),
                typeof(Inc4)
            };

            _pool1 = world.GetPool<Inc1>();
            _pool1.OnResize += _pool_OnResize;
            _pool2 = world.GetPool<Inc2>();
            _pool2.OnResize += _pool_OnResize;
            _pool3 = world.GetPool<Inc3>();
            _pool3.OnResize += _pool_OnResize;
            _pool4 = world.GetPool<Inc4>();
            _pool4.OnResize += _pool_OnResize;

            _incComponents1 = _pool1.Components;
            _incComponents2 = _pool2.Components;
            _incComponents3 = _pool3.Components;
            _incComponents4 = _pool4.Components;

            _get1 = new int[world.Config.ViewEntityCacheSize];
            _get2 = new int[world.Config.ViewEntityCacheSize];
            _get3 = new int[world.Config.ViewEntityCacheSize];
            _get4 = new int[world.Config.ViewEntityCacheSize];
        }

        public override void AddEntity(in Entity entity)
        {
            if (TryBufferingOperationIfLocked(true, entity)) return;
            if (Entities.Length == EntitiesIndex)
            {
                var newLength = EntitiesIndex * 2;
                Array.Resize(ref Entities, newLength);
                Array.Resize(ref _get1, newLength);
                Array.Resize(ref _get2, newLength);
                Array.Resize(ref _get3, newLength);
                Array.Resize(ref _get4, newLength);
            }

            ref var entityData = ref entity.World.GetEntityData(entity);
            for (int i = 0, left = 4; left > 0 && i < entityData.ComponentIndex; i++)
            {
                if (entityData.ComponentTypes[i] == ComponentType<Inc1>.TypeId)
                {
                    _get1[EntitiesIndex] = entityData.ComponentIds[i];
                    left--;
                    continue;
                }
                if (entityData.ComponentTypes[i] == ComponentType<Inc2>.TypeId)
                {
                    _get2[EntitiesIndex] = entityData.ComponentIds[i];
                    left--;
                    continue;
                }
                if (entityData.ComponentTypes[i] == ComponentType<Inc3>.TypeId)
                {
                    _get3[EntitiesIndex] = entityData.ComponentIds[i];
                    left--;
                }
                if (entityData.ComponentTypes[i] == ComponentType<Inc4>.TypeId)
                {
                    _get4[EntitiesIndex] = entityData.ComponentIds[i];
                    left--;
                }
            }

            EntitiesMap[entity.Id] = EntitiesIndex;
            Entities[EntitiesIndex++] = entity;
        }

        public override void RemoveEntity(in Entity entity)
        {
            if (TryBufferingOperationIfLocked(false, entity)) return;
            var id = EntitiesMap[entity.Id];
            EntitiesMap.Remove(entity.Id);
            EntitiesIndex--;

            //Replace removed entity with newest one.
            if (id < EntitiesIndex)
            {
                Entities[id] = Entities[EntitiesIndex];
                EntitiesMap[Entities[id].Id] = id;

                _get1[id] = _get1[EntitiesIndex];
                _get2[id] = _get2[EntitiesIndex];
                _get3[id] = _get3[EntitiesIndex];
                _get4[id] = _get4[EntitiesIndex];
            }
        }

        private void _pool_OnResize()
        {
            _incComponents1 = _pool1.Components;
            _incComponents2 = _pool2.Components;
            _incComponents3 = _pool3.Components;
            _incComponents4 = _pool4.Components;
        }

        IEnumerator<ViewEntity<Inc1, Inc2, Inc3, Inc4>> IEnumerable<ViewEntity<Inc1, Inc2, Inc3, Inc4>>.GetEnumerator()
        {
            return new Enumerator(this);
        }

        new public IEnumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        new public class Enumerator : BaseViewEnumerator<ViewEntity<Inc1, Inc2, Inc3, Inc4>, EcsView<Inc1, Inc2, Inc3, Inc4>>
        {
            public Enumerator(EcsView<Inc1, Inc2, Inc3, Inc4> view) : base(view)
            {
            }

            public override ViewEntity<Inc1, Inc2, Inc3, Inc4> Current
            {
                get
                {
                    var viewEntity = new ViewEntity<Inc1, Inc2, Inc3, Inc4>();
                    viewEntity._view = _view;
                    viewEntity.Entity = _index;

                    return viewEntity;
                }
            }
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
