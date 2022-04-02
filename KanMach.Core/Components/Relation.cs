using KanMach.Core.Ecs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanMach.Core.Components
{
    public struct Relation
    {

        public Entity? Parent;
        public Entity? FirstChild;
        public Entity? Next;
        public Entity? Previous;

    }
}
