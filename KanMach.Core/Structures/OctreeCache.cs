using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanMach.Core.Structures
{
    internal class OctreeCache<T>
    {

        private Dictionary<int, OctreeLeaf<T>> _cachedLeafs;
        private List<OctreeNode<T>> _nodePool;

        public OctreeCache()
        {
            _cachedLeafs = new Dictionary<int, OctreeLeaf<T>>();
            _nodePool = new List<OctreeNode<T>>();
        }

    }
}
