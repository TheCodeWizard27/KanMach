using KanMach.Core.Structures.Collision;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanMach.Core.Structures
{
    public class Octree<T>
    {
        internal OctreeCache<T> _cache;
        public OctreeNode<T> CurrentRoot { get; protected set; }

        public Octree(BoundingBox boundingBox, int maxChildren = 4)
        {
            CurrentRoot = new OctreeNode<T>(boundingBox, maxChildren);
            _cache = new OctreeCache<T>();
        }

        public void AddItem(BoundingBox itemBounds, T item)
        {
            CurrentRoot.AddItem(itemBounds, item);
        }

    }
}
