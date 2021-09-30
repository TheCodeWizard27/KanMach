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
        internal EcsConfig Config;

        internal IComponentPool[] ComponentPools;
        internal GrowList<EcsView> Views;

        internal GrowList<int> FreeEntityIds;
        internal EntityData[] Entities;
        internal int EntityIndex;

        public EcsWorld(EcsConfig config = null)
        {
            _config = config ?? EcsConfig.Default;

            ComponentPools = new IComponentPool[_config.WorldComponentPoolsCacheSize];
            Views = new GrowList<EcsView>(_config.ViewCacheSize);

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
        public void Recycle(ref EntityData entityData)
        {
            FreeEntityIds.Add(entityData.Id);
        }

        public T View<T>() where T : EcsView
        {
            for (int i = 0; i < Views.Index; i++)
            {
                if (Views.Items[i].GetType() != typeof(T)) continue;

                return (T)Views.Items[i];
            }

            var view = Activator.CreateInstance(typeof(T), this);
            
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void RemoveComponentsFromView(int typeId, in Entity entity, in EntityData entityData)
        {

        }
        internal void AddComponentsToView(int typeId, in Entity entity, in EntityData entityData)
        {

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

    }
}
