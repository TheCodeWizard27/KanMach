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
        private List<OctreeNode<T>> _children;
        
        public OctreeNode<T> Parent { get; internal set; } 
        public BoundingBox BoundingBox { get; internal set; }

        public int MaxLeafs { get; private set; }

        internal OctreeNode(BoundingBox boundingBox, OctreeCache<T> cache, int maxLeafs)
        {
            _cache = cache;
            MaxLeafs = maxLeafs;

            _children = new List<OctreeNode<T>>();
            _leafs = new List<OctreeLeaf<T>>();
        }

        internal OctreeLeaf<T> AddItem(BoundingBox itemBounds, T item)
        {
            var leaf = new OctreeLeaf<T>(itemBounds, item);
            if (TryToAddLeaf(leaf))
                ResizeRootNode();

            return leaf;
        }

        internal bool TryToAddLeaf(OctreeLeaf<T> leaf)
        {
            if(!leaf.BoundingBox.Contains(BoundingBox))
            {
                return false;
            }

            if(_leafs.Count >= MaxLeafs && !_children.Any())
            {
                SplitNode();
                // Split Node up and then try to add it.
            }
            else if(_children.Count > 0)
            {
                // TODO GO through all children and try to add it
            }

            _leafs.Add(leaf);
            return true;
        }

        private void SplitNode(OctreeNode<T> existingNode = null)
        {

        }

        private void ResizeRootNode()
        {
            // Get expand direction. Expand 0.5 in front and back of direction. split root.
        }

    }
}
