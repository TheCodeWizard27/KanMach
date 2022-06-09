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
        private BoundingBox _boundingBox;

        public BoundingBox BoundingBox
        {
            get { return _boundingBox; }
            set {

                if (value == _boundingBox)
                    Node.AddChange(this);

                _boundingBox = value;
            }
        }

        public T Item { get; set; }

        public OctreeNode<T> Node { get; internal set; }

        internal OctreeLeaf(BoundingBox boundingBox, T item)
        {
            _boundingBox = boundingBox;
            Item = item;
        }

    }
}
