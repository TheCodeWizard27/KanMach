using KanMach.Core.Ecs.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace KanMach.Core.Ecs
{

    public class ComponentPool<T> : IComponentPool where T : struct
    {

        public T[] Components;
        public int ComponentIndex;
        public GrowList<int> _freeComponents;

        public Type ItemType => typeof(T);

        public delegate void OnResizeHandler();
        public event OnResizeHandler OnResize;

        public ComponentPool()
        {
            Components = new T[128];
            _freeComponents = new GrowList<int>(128);
        }

        public void CopyData(int srcId, int targetId)
        {
            Components[targetId] = Components[srcId];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T GetItem(int id) => ref Components[id];

        public int New()
        {
            if(_freeComponents.Index > 0)
            {
                return _freeComponents.Take();
            }

            var id = ComponentIndex;
            if(Components.Length == id)
            {
                Array.Resize(ref Components, ComponentIndex * 2);
                OnResize?.Invoke();
            }
            ComponentIndex++;
            return id;
        }

        public void Recycle(int id)
        {
            _freeComponents.Add(id);
            Components[id] = default;
        }
    }
}
