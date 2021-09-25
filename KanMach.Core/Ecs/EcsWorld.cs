using KanMach.Core.Ecs.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace KanMach.Core.Ecs
{
    public class EcsWorld
    {
        private EcsConfig _config;

        internal IComponentPool[] ComponentPools;

        internal GrowList<int> FreeEntityIds;
        internal EntityData[] Entities;
        internal int EntityIndex;

        public EcsWorld(EcsConfig config = null)
        {
            _config = config ?? EcsConfig.Default;

            ComponentPools = new IComponentPool[_config.WorldComponentPoolsCacheSize];

            Entities = new EntityData[_config.WorldEntitiesCacheSize];
            FreeEntityIds = new GrowList<int>(_config.WorldEntitiesCacheSize);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref EntityData GetEntityData(in Entity entity) => ref Entities[entity.Id];

        public Entity NewEntity()
        {
            Entity entity;
            entity.World = this;
            entity.Gen = 0;
            
            if(FreeEntityIds.Index > 0)
            {
                entity.Id = FreeEntityIds.Take();
                ref var entityData = ref Entities[entity.Id];
                entity.Gen = entityData.Gen++;
            }else
            {
                if(EntityIndex == Entities.Length)
                {
                    Array.Resize(ref Entities, Entities.Length * 2);
                }
                entity.Id = EntityIndex++;
                ref var entityData = ref Entities[entity.Id];
                entityData.ComponentIds = new int[_config.EntityComponentCacheSize];
                entityData.ComponentTypes = new int[_config.EntityComponentCacheSize];
            }

            return entity;
        }

        internal ComponentPool<T> GetPool<T>() where T : struct
        {
            var typeId = ComponentType<T>.TypeId;
            if(ComponentPools.Length < typeId)
            {
                var len = ComponentPools.Length *1;
                while (len <= typeId)
                {
                    len *= 2;
                }
                Array.Resize(ref ComponentPools, len);
            }

            var pool = ComponentPools[typeId];
            if(pool == null)
            {
                pool = ComponentPools[typeId] = new ComponentPool<T>();
            }

            return (ComponentPool<T>) pool;
        }

        public void Recycle(ref EntityData entityData)
        {
            FreeEntityIds.Add(entityData.Id);
        }

    }
}
