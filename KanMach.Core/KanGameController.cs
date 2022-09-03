using KanMach.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanMach.Core
{
    public abstract class KanGameController : IDisposable
    {

        public IKanContext Context { get; internal set; }

        public abstract void Init();

        public abstract void Update(FrameTime delta);

        public abstract void Dispose();

    }
}
