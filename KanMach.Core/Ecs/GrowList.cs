using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace KanMach.Core.Ecs
{
    public class GrowList<T>
    {
        private T[] _items;

        public T[] Items { get => _items; }
        public int Index { get; set; }

        public T this[int i]
        {
            get => Items[i];
            set => Items[i] = value;
        }

        public GrowList(int capacity)
        {
            _items = new T[capacity];
        }

        public void Add(T item)
        {
            if(Items.Length == Index)
            {
                Array.Resize(ref _items, Items.Length * 2);
            }

            _items[Index++] = item;
        }

        /// <summary>
        /// Takes top element and reduces index by one.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Take() => _items[--Index];

    }
}
