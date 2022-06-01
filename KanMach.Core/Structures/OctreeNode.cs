using KanMach.Core.Structures.Collision;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
            BoundingBox = boundingBox;

            _children = new List<OctreeNode<T>>();
            _leafs = new List<OctreeLeaf<T>>();
        }

        internal OctreeNode<T> AddItem(OctreeLeaf<T> leaf)
        {
            var root = this;
            
            if (!TryToAddLeaf(leaf))
                root = ResizeRootNode(leaf);

            return root;
        }

        /// <summary>
        /// Tries to add a leaf and automatically splits itself if too many leafs are present.
        /// </summary>
        /// <param name="leaf"></param>
        /// <returns>
        /// If the leaf was successfully added return true and 
        /// false if the leaf is outside of the bounds of the root node.
        /// </returns>
        internal bool TryToAddLeaf(OctreeLeaf<T> leaf)
        {
            if(!BoundingBox.Contains(leaf.BoundingBox))
            {
                return false;
            }

            if(_leafs.Count >= MaxLeafs)
                SplitNode();

            if (_children.Any())
            {
                var node = GetContainingNode(leaf.BoundingBox);
                if (node != null)
                {
                    node.TryToAddLeaf(leaf);
                    return true;
                }
            }

            _leafs.Add(leaf);
            return true;
        }

        private OctreeNode<T> GetContainingNode(BoundingBox bounds)
        {
            foreach(OctreeNode<T> node in _children)
            {
                if (node.BoundingBox.Contains(bounds)) return node;
            }

            return null;
        }

        private void SplitNode(OctreeNode<T> existingNode = null)
        {
            var center = BoundingBox.Center;
            var newDimension = BoundingBox.Dimensions / 4;

            for(var x = -1f; x <= 1f; x += 2f)
            {
                for(var y = -1f; y <= 1f; y += 2f)
                {
                    for(var z = -1f; z <= 1f; z += 2f)
                    {
                        var nodeCenter = center + (newDimension * new Vector3(x, y, z));
                        var min = nodeCenter - newDimension;
                        var max = nodeCenter + newDimension;

                        var nodeBounds = new BoundingBox(min, max);

                        OctreeNode<T> node;
                        if (existingNode?.BoundingBox == nodeBounds)
                        {
                            node = existingNode;
                        }
                        else
                        {
                            node = new OctreeNode<T>(nodeBounds, _cache, MaxLeafs);
                        }

                        _children.Add(node);
                        node.Parent = this;
                    }
                }
            }

            // Redistribute existing leafs.
            for(var i = 0; i < _leafs.Count; i++)
            {
                foreach (var node in _children)
                {
                    if (node.TryToAddLeaf(_leafs[i]))
                    {
                        i--;
                        _leafs.Remove(_leafs[i]);
                        break;
                    }
                }
            }

        }

        private OctreeNode<T> ResizeRootNode(OctreeLeaf<T> leaf)
        {
            // Get expand direction. Expand 0.5 in front and back of direction. split root.
            var oldRoot = this;

            var expandDirection = Vector3.Normalize(leaf.BoundingBox.Center - oldRoot.BoundingBox.Center);
            var newCenter = oldRoot.BoundingBox.Center;
            var halfDimension = oldRoot.BoundingBox.Dimensions / 2;

            // Get a new center that is on one the corners of the current root node.
            newCenter.X += expandDirection.X >= 0 ? halfDimension.X : -halfDimension.X;
            newCenter.Y += expandDirection.Y >= 0 ? halfDimension.Y : -halfDimension.Y;
            newCenter.Z += expandDirection.Z >= 0 ? halfDimension.Z : -halfDimension.Z;
            var newMin = newCenter - halfDimension * 2f;
            var newMax = newCenter + halfDimension * 2f;
            var newRoot = new OctreeNode<T>(new BoundingBox(newMin, newMax), _cache, MaxLeafs);
            newRoot.SplitNode(oldRoot);
            newRoot.TryToAddLeaf(leaf);

            return newRoot;
        }

    }
}
