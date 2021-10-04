using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanMach.Core.Ecs
{
    public struct EntityData
    {

        public int Id;
        public int Gen;

        public int[] ComponentIds;
        public int[] ComponentTypes;
        public int ComponentIndex;

        public EcsWorld World;

    }
}
