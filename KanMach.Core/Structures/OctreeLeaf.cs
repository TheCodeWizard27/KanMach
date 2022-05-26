using KanMach.Core.Structures.Collision;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanMach.Core.Structures
{
    public class OctreeLeaf<T>
    {

        public BoundingBox BoundingBox { get; set; }

        public T Item { get; set; }

        internal OctreeLeaf(BoundingBox boundingBox, T item)
        {
            BoundingBox = boundingBox;
            Item = item;
        }

    }
}
