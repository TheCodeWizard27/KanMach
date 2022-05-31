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
            _cache = new OctreeCache<T>();
            CurrentRoot = new OctreeNode<T>(boundingBox, _cache, maxChildren);
        }

        public OctreeLeaf<T> AddItem(BoundingBox itemBounds, T item)
        {
            var leaf = new OctreeLeaf<T>(itemBounds, item);
            CurrentRoot = CurrentRoot.AddItem(leaf);
            return leaf;
        }

    }
}
