using KanMach.Core.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanMach.Core.Ecs.Extensions
{
    public static class EntityHierarchyExtension
    {

        public static void Attach(in this Entity parentEntity, in Entity newChild)
        {
            ref var parentRelation = ref parentEntity.Get<Relation>();

            newChild.Detach();

            ref var newChildRelation = ref newChild.Get<Relation>();
            newChildRelation.Parent = parentEntity;
            newChildRelation.Previous = null;
            newChildRelation.Next = null;

            if(!parentRelation.FirstChild.HasValue)
            {
                parentRelation.FirstChild = newChild;
                return;
            }

            var nextChild = parentRelation.FirstChild;
            ref var nextChildRelation = ref nextChild.Value.Get<Relation>();

            while(nextChildRelation.Next.HasValue)
            {
                nextChild = nextChildRelation.Next;
                nextChildRelation = ref nextChild.Value.Get<Relation>();
            }

            newChildRelation.Previous = nextChild;
            nextChildRelation.Next = newChild;
        }

        public static void Detach(in this Entity entity)
        {
            if (!entity.Has<Relation>()) return;

            ref var relation = ref entity.Get<Relation>();

            if (!relation.Previous.HasValue && relation.Parent.HasValue)
            {
                ref var parentRelation = ref relation.Parent.Value.Get<Relation>();
                parentRelation.FirstChild = relation.Next;
            }

            if(relation.Previous.HasValue)
            {
                ref var previousRelation = ref relation.Previous.Value.Get<Relation>();
                previousRelation.Next = relation.Next;
            }

            if (relation.Next.HasValue)
            {
                ref var nextRelation = ref relation.Next.Value.Get<Relation>();
                nextRelation.Previous = relation.Previous;
            }
        }

    }
}
