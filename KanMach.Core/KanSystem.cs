using KanMach.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanMach.Core
{
    public abstract class KanSystem
    {

        public abstract void Init();

        public abstract void Run(TimeSpan delta);

        public abstract void Dispose();

    }
}
