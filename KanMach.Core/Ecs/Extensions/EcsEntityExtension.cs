using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanMach.Core.Ecs.Extensions
{
    public static class EcsEntityExtension
    {

        public static ref T Get<T>(in this Entity entity) where T : struct
        {
            ref var entityData = ref entity.World.GetEntityData(entity);
            
            var typeId = ComponentType<T>.TypeId;

            for (var i = 0; i < entityData.ComponentIndex; i++)
            {
                if (entityData.ComponentTypes[i] != typeId) continue;
                var pool = (ComponentPool<T>) entity.World.ComponentPools[typeId];
                return ref pool.GetItem(i);
            }

            if(entityData.ComponentIndex == entityData.ComponentIds.Length)
            {
                Array.Resize(ref entityData.ComponentIds, entityData.ComponentIndex *2);
                Array.Resize(ref entityData.ComponentTypes, entityData.ComponentIndex *2);
            }
            var componentPool = entity.World.GetPool<T>();
            var id = componentPool.New();

            entityData.ComponentTypes[entityData.ComponentIndex] = typeId;
            entityData.ComponentIds[entityData.ComponentIndex] = id;
            entityData.ComponentIndex++;

            entity.World.AddComponentsToView(typeId, entity, entityData);

            return ref componentPool.GetItem(id);
        }

        public static bool Has<T>(in this Entity entity) where T : struct
        {
            ref var entityData = ref entity.World.GetEntityData(entity);

            var typeId = ComponentType<T>.TypeId;
            for (var i = 0; i < entityData.ComponentIndex; i++)
            {
                if (entityData.ComponentTypes[i] == typeId) return true;
            }

            return false;
        }

        public static void Del<T>(in this Entity entity) where T : struct
        {
            ref var entityData = ref entity.World.GetEntityData(entity);

            var typeId = ComponentType<T>.TypeId;
            for (var i = 0; i < entityData.ComponentIndex; i++)
            {
                if (entityData.ComponentTypes[i] != typeId) continue;

                entity.World.RemoveComponentsFromView(typeId, entity, entityData);

                entity.World.ComponentPools[typeId].Recycle(i);
                entityData.ComponentIndex--;
                if(i < entityData.ComponentIndex)
                {
                    entityData.ComponentIds[i] = entityData.ComponentIds[entityData.ComponentIndex];
                    entityData.ComponentTypes[i] = entityData.ComponentTypes[entityData.ComponentIndex];
                }

                break;
            }
        }

        public static void Destroy(in this Entity entity)
        {
            ref var entityData = ref entity.World.GetEntityData(entity);

            for (var i = 0; i < entityData.ComponentIndex; i++)
            {
                entity.World.RemoveComponentsFromView(entityData.ComponentTypes[i], entity, entityData);
                entity.World.ComponentPools[entityData.ComponentTypes[i]].Recycle(entityData.ComponentIds[i]);
            }
            entityData.ComponentIndex = 0;

            entity.World.Recycle(ref entityData);
        }

    }
}
