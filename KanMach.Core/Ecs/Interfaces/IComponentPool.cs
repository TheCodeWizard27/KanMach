using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanMach.Core.Ecs.Interfaces
{
    interface IComponentPool
    {
        Type ItemType { get; }
        void Recycle(int id);
        int New();
        void CopyData(int srcIdx, int dstIdx);
    }
}
