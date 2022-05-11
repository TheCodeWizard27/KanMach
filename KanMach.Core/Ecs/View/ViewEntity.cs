using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanMach.Core.Ecs.View
{
    public struct ViewEntity<T> where T : struct
    {
        internal EcsView<T> _view;

        public int Entity;

        public ref T Component => ref _view._incComponents1[_view._get1[Entity]];

    }

    public struct ViewEntity<T, T2> 
        where T : struct 
        where T2 : struct
    {
        internal EcsView<T, T2> _view;

        public int Entity;

        public ref T Component1 => ref _view._incComponents1[_view._get1[Entity]];
        public ref T2 Component2 => ref _view._incComponents2[_view._get2[Entity]];

    }

    public struct ViewEntity<T, T2, T3>
        where T : struct
        where T2 : struct
        where T3 : struct
    {
        internal EcsView<T, T2, T3> _view;

        public int Entity;

        public ref T Component1 => ref _view._incComponents1[_view._get1[Entity]];
        public ref T2 Component2 => ref _view._incComponents2[_view._get2[Entity]];
        public ref T3 Component3 => ref _view._incComponents3[_view._get3[Entity]];
    }

    public struct ViewEntity<T, T2, T3, T4>
        where T : struct
        where T2 : struct
        where T3 : struct
        where T4 : struct
    {
        internal EcsView<T, T2, T3, T4> _view;

        public int Entity;

        public ref T Component1 => ref _view._incComponents1[_view._get1[Entity]];
        public ref T2 Component2 => ref _view._incComponents2[_view._get2[Entity]];
        public ref T3 Component3 => ref _view._incComponents3[_view._get3[Entity]];
        public ref T4 Component4 => ref _view._incComponents4[_view._get4[Entity]];
    }
}
