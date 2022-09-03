using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanMach.Core
{
    public struct FrameTime
    {

        public TimeSpan TimeSpan;
        public float Milliseconds { get => (float)TimeSpan.Ticks / 10000; }

        public FrameTime(TimeSpan timespan)
        {
            TimeSpan = timespan;
        }

        public float GetDelta(int frameRate)
        {
            return Milliseconds / (1000 / frameRate);
        }

    }
}
