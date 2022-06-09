using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanMach.Core.Structures
{
    internal class OctreeCache<T>
    {

        private HashSet<OctreeLeaf<T>> _pendingChanges;

        private Dictionary<int, OctreeLeaf<T>> _cachedLeafs;
        private List<OctreeNode<T>> _nodePool;

        public OctreeCache()
        {
            _cachedLeafs = new Dictionary<int, OctreeLeaf<T>>();
            _nodePool = new List<OctreeNode<T>>();
            _pendingChanges = new HashSet<OctreeLeaf<T>>();
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

    }
}
