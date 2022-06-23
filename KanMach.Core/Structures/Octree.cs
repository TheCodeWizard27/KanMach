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

        public Dictionary<int, OctreeLeaf<T>> CachedLeafs { get; internal set; }
        public OctreeNode<T> CurrentRoot { get; protected set; }

        public Octree(BoundingBox boundingBox, int maxChildren = 4)
        {
            _cache = new OctreeCache<T>();
            CachedLeafs = new Dictionary<int, OctreeLeaf<T>>();
            CurrentRoot = new OctreeNode<T>(boundingBox, _cache, maxChildren);
        }

        public OctreeLeaf<T> AddLeaf(BoundingBox itemBounds, T item)
        {
            var leaf = _cache.NewLeaf(itemBounds, item);

            CurrentRoot = CurrentRoot.AddLeaf(leaf);
            return leaf;
        }

        public OctreeLeaf<T> AddLeaf(int id, BoundingBox itemBounds, T item)
        {
            var leaf = AddLeaf(itemBounds, item);
            CachedLeafs.Add(id, leaf);
            return leaf;
        }

        public void RemoveLeaf(int id)
        {
            RemoveLeaf(CachedLeafs[id]);

            CachedLeafs.Remove(id);
        }

        public void RemoveLeaf(OctreeLeaf<T> leaf)
        {
            var node = leaf.Node;
            node.RemoveLeaf(leaf);

            if(node.Parent != null)
            {
                node.ConsiderConsolidation();
            }
        }

        public void ResolveChanges()
        {
            foreach(var leaf in _cache.CollectChanges())
            {
                CurrentRoot = leaf.Node.ResolveChanges(leaf);
            }

            CurrentRoot = CurrentRoot.TryTrimChildren();
        }

    }
}
