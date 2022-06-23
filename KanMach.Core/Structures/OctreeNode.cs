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
        
        private List<OctreeLeaf<T>> _leafs = new List<OctreeLeaf<T>>();
        private List<OctreeNode<T>> _children = new List<OctreeNode<T>>();
        
        public OctreeNode<T> Parent { get; internal set; } 
        public BoundingBox BoundingBox { get; internal set; }

        public int MaxLeafs { get; private set; }

        internal OctreeNode(BoundingBox boundingBox, OctreeCache<T> cache, int maxLeafs)
        {
            Init(boundingBox, cache, maxLeafs);
        }

        // Moving constructor code to a method makes handling pools easier.
        internal OctreeNode<T> Init(BoundingBox boundingBox, OctreeCache<T> cache, int maxLeafs)
        {
            _cache = cache;
            Parent = null;
            MaxLeafs = maxLeafs;
            BoundingBox = boundingBox;

            _children.Clear();
            _leafs.Clear();

            return this;
        }

        public int GetTotalLeafs()
        {
            var leafCount = _leafs.Count;
            foreach(var child in _children)
            {
                leafCount += child.GetTotalLeafs();
            }

            return leafCount;
        }

        internal OctreeNode<T> AddLeaf(OctreeLeaf<T> leaf)
        {
            var root = this;
            
            if (!TryToAddLeaf(leaf)) 
                root = ResizeRootAndAdd(leaf);

            return root;
        }

        internal void RemoveLeaf(OctreeLeaf<T> leaf)
        {
            _leafs.Remove(leaf);
            _cache.RecycleLeaf(leaf);
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
            leaf.Node = this;
            return true;
        }

        internal void AddChange(OctreeLeaf<T> changedLeaf)
        {
            _cache.AddChange(changedLeaf);
        }

        internal OctreeNode<T> ResolveChanges(OctreeLeaf<T> leaf)
        {
            if (BoundingBox.Contains(leaf.BoundingBox))
            {
                foreach (var child in _children)
                {
                    if (child.TryToAddLeaf(leaf))
                    {
                        _leafs.Remove(leaf);
                        break;
                    }
                }
            }
            else
            {
                _leafs.Remove(leaf);

                if (Parent == null)
                    return ResizeRootAndAdd(leaf);

                Parent.TryToAddLeaf(leaf);
            }

            return this;
        }

        internal OctreeNode<T> TryTrimChildren()
        {
            if (_leafs.Any())
            {
                OctreeNode<T> loneChild = null;
                foreach (var child in _children)
                {
                    if (child.GetTotalLeafs() == 0) continue;

                    if (loneChild != null) return this;

                    loneChild = child;
                }

                if(loneChild != null)
                {
                    foreach(var child in _children)
                    {
                        if (child == loneChild) continue;

                        child.Recycle();
                    }
                }

                Recycle(false);
                loneChild.Parent = null;
                return loneChild;
            }

            return this;
        }

        internal void ConsiderConsolidation()
        {
            if(_children.Count > 0 && GetTotalLeafs() < MaxLeafs)
            {
                ConsolidateChildren();
                Parent?.ConsiderConsolidation();
            }
        }

        internal void Recycle(bool recycleChildren = true)
        {
            if(recycleChildren) RecycleChildren();

            _cache.RecycleNode(this);
        }

        internal void RecycleChildren()
        {
            foreach(var child in _children)
            {
                child.Recycle();
            }
        }

        private void ConsolidateChildren()
        {
            foreach(var child in _children)
            {
                child.ConsolidateChildren();

                foreach(var leaf in child._leafs)
                {
                    _leafs.Add(leaf);
                    leaf.Node = this;
                }
            }
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
                            node = _cache.NewNode(nodeBounds, MaxLeafs);
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

        private OctreeNode<T> ResizeRootAndAdd(OctreeLeaf<T> leaf)
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

            var newRoot = _cache.NewNode(new BoundingBox(newMin, newMax), MaxLeafs);
            newRoot.SplitNode(oldRoot);
            newRoot.TryToAddLeaf(leaf);

            return newRoot;
        }

    }
}
