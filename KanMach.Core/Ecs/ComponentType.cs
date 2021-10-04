using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KanMach.Core.Ecs
{

    internal sealed class ComponentType
    {
        internal static int ComponentTypeIndex;
    }

    public static class ComponentType<T> where T : struct
    {

        public static readonly int TypeId;
        public static readonly Type Type;

        static ComponentType()
        {
            TypeId = Interlocked.Increment(ref ComponentType.ComponentTypeIndex);
            Type = typeof(T);
        }

    }
}
