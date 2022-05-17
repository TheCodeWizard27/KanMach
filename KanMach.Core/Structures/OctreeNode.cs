using KanMach.Core.Structures.Collision;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanMach.Core.Structures
{
    public class OctreeNode<T>
    {
        private const int NumChildNodes = 8;

        private OctreeCache<T> _cache;
        
        private List<OctreeLeaf<T>> _leafs;
        private OctreeNode<T> _children;
        
        public OctreeNode<T> Parent { get; internal set; } 
        public BoundingBox BoundingBox { get; internal set; }

        internal OctreeNode(BoundingBox boundingBox, OctreeCache<T> cache, int MaxChildren)
        {
            _cache = cache;
        }

        internal void AddItem<T>(BoundingBox itemBounds, T item)
        {
            
        }
    }
}
