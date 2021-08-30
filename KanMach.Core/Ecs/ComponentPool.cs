using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanMach.Core.Ecs
{

    interface IComponentPool
    {
        Type ItemType { get; }
        object GetItem(int idx);
        void Recycle(int idx);
        int New();
        void CopyData(int srcIdx, int dstIdx);
    }

    class ComponentPool<T> : IComponentPool where T : struct
    {

        public List<T> Components;

        public Type ItemType => throw new NotImplementedException();

        public void CopyData(int srcIdx, int dstIdx)
        {
            throw new NotImplementedException();
        }

        public object GetItem(int idx)
        {
            throw new NotImplementedException();
        }

        public int New()
        {
            throw new NotImplementedException();
        }

        public void Recycle(int idx)
        {
            throw new NotImplementedException();
        }
    }
}
