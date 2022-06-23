using KanMach.Core.Structures.Collision;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanMach.Core.Structures
{
    internal class OctreeCache<T>
    {

        private HashSet<OctreeLeaf<T>> _pendingChanges;
        
        private ConcurrentBag<OctreeNode<T>> _nodePool;
        private ConcurrentBag<OctreeLeaf<T>> _leafPool;

        public OctreeCache()
        {
            _pendingChanges = new HashSet<OctreeLeaf<T>>();

            _nodePool = new ConcurrentBag<OctreeNode<T>>();
            _leafPool = new ConcurrentBag<OctreeLeaf<T>>();
        }

        public void AddChange(OctreeLeaf<T> changedLeaf)
        {
            _pendingChanges.Add(changedLeaf);
        }

        public List<OctreeLeaf<T>> CollectChanges()
        {
            var changes = _pendingChanges.ToList();
            _pendingChanges.Clear();
            return changes;
        }

        internal void RecycleLeaf(OctreeLeaf<T> leaf)
        {
            _leafPool.Add(leaf);
        }
        internal void RecycleNode(OctreeNode<T> node)
        {
            _nodePool.Add(node);
        }

        internal OctreeLeaf<T> NewLeaf(BoundingBox boundingBox, T item)
        {
            if(_leafPool.TryTake(out var leaf))
            {
                leaf.Init(boundingBox, item);
            } else
            {
                leaf = new OctreeLeaf<T>(boundingBox, item);
            }

            return leaf;
        }

        internal OctreeNode<T> NewNode(BoundingBox boundingBox, int maxLeafs)
        {
            return _nodePool.TryTake(out var node) 
                ? node.Init(boundingBox, this, maxLeafs) 
                : node = new OctreeNode<T>(boundingBox, this, maxLeafs);
        }

    }
}
