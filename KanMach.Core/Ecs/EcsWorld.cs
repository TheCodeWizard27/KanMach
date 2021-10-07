using KanMach.Core.Ecs.Interfaces;
using KanMach.Core.Ecs.View;
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
        internal Dictionary<int, GrowList<EcsView>> ViewIncludesMap;
        internal Dictionary<int, GrowList<EcsView>> ViewExcludesMap;

        internal GrowList<int> FreeEntityIds;
        internal EntityData[] Entities;
        internal int EntityIndex;

        public EcsWorld(EcsConfig config = null)
        {
            Config = config ?? EcsConfig.Default;

            ComponentPools = new IComponentPool[Config.WorldComponentPoolsCacheSize];
            
            Views = new GrowList<EcsView>(Config.ViewCacheSize);
            ViewIncludesMap = new Dictionary<int, GrowList<EcsView>>(Config.EntityComponentCacheSize);
            ViewExcludesMap = new Dictionary<int, GrowList<EcsView>>(Config.EntityComponentCacheSize);

            Entities = new EntityData[Config.WorldEntitiesCacheSize];
            FreeEntityIds = new GrowList<int>(Config.WorldEntitiesCacheSize);
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
                entityData.ComponentIds = new int[Config.EntityComponentCacheSize];
                entityData.ComponentTypes = new int[Config.EntityComponentCacheSize];
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
                // Return already existing view.
                if (Views.Items[i].GetType() != typeof(T)) continue;

                return (T)Views.Items[i];
            }

            var view = (T) Activator.CreateInstance(typeof(T), this);

            Views.Add(view);

            for(var i = 0; i < view.IncludedTypeIndices.Length; i++)
            {
                if(!ViewIncludesMap.TryGetValue(view.IncludedTypeIndices[i], out var views))
                {
                    views = new GrowList<EcsView>(Config.ViewCacheSize);
                    ViewIncludesMap[view.IncludedTypeIndices[i]] = views;
                }
                views.Add(view);
            }
            if(view.ExcludedTypeIndices != null)
            {
                for(var i = 0; i < view.ExcludedTypeIndices.Length; i++)
                {
                    if(!ViewExcludesMap.TryGetValue(view.ExcludedTypeIndices[i], out var views))
                    {
                        views = new GrowList<EcsView>(Config.ViewCacheSize);
                        ViewExcludesMap[view.ExcludedTypeIndices[i]] = views;
                    }
                    views.Add(view);
                }
            }

            return view;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void RemoveComponentsFromView(int typeId, in Entity entity, in EntityData entityData)
        {

        }
        internal void AddComponentsToView(int typeId, in Entity entity, in EntityData entityData)
        {

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ComponentPool<T> GetPool<T>() where T : struct
        {
            var typeId = ComponentType<T>.TypeId;
            if(ComponentPools.Length < typeId)
            {
                // Increase size of ComponentPools buffer till it can store
                // every component type.
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
