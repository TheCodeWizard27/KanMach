using KanMach.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanMach.Sample
{
    public class SampleSystem : KanSystem
    {

        public Action InitAction { get; set; }
        public Action<FrameTime> RunAction { get; set; }
        public Action DisposeAction { get; set; }


        public override void Init()
        {
            InitAction?.Invoke();
        }

        public override void Run(FrameTime delta)
        {
            RunAction?.Invoke(delta);
        }

        public override void Dispose()
        {
            DisposeAction?.Invoke();
        }

    }
}
