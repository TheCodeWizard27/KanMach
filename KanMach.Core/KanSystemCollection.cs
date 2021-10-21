using KanMach.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanMach.Core
{
    public class KanSystemEntry
    {
        public int Priority { get; set; }
        public KanSystem System { get; set; }

        public KanSystemEntry(int priority, KanSystem system)
        {
            Priority = priority;
            System = system;
        }

    }

    public class KanSystemCollection
    {

        protected IKanContext Context { get; private set; }
        protected List<KanSystemEntry> Systems { get; private set; } = new List<KanSystemEntry>();

        public KanSystemCollection(IKanContext context)
        {
            Context = context;
        }

        public void Run(TimeSpan delta)
        {
            Systems.ForEach(item => item.System.Run(delta));
        }

        public Type Add<Type>(Action<Type> configure) where Type : KanSystem
        {
            return Add(0, configure);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Type"></typeparam>
        /// <param name="priority">Priority defines in which order the systems are executed. The higher the number the </param>
        /// <returns></returns>
        public Type Add<Type>(int priority = 0, Action<Type> configure = null) where Type : KanSystem
        {
            var system = (Type) ActivatorUtilities.CreateInstance(Context.Provider, typeof(Type));
            Systems.Add(new KanSystemEntry(priority, system));
            configure?.Invoke(system);
            system.Init();

            Systems.Sort((item, item2) => item.Priority.CompareTo(item2.Priority));

            return system;
        }

    }
}
