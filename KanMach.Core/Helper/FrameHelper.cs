using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KanMach.Core.Helper
{
    public static class FrameHelper
    {

        public static void SleepPrecise(int frameRate, TimeSpan duration)
        {
            SleepPrecise((1000 / frameRate) - (duration.Ticks / 10000));
        }

        // The following code was taken from the following link:
        // https://blat-blatnik.github.io/computerBear/making-accurate-sleep-function/
        public static void SleepPrecise(double milliseconds)
        {
            if (milliseconds <= 0) return;

            var estimate = 5d;
            var mean = 5d;
            var m2 = 0d;
            var count = 1L;

            DateTime begin;

            while (milliseconds > estimate)
            {
                begin = DateTime.Now;
                Thread.Sleep(TimeSpan.FromMilliseconds(1));
                var observed = (double)(DateTime.Now - begin).Ticks / 10000d;
                milliseconds -= observed;

                count++;
                var delta = observed - mean;
                mean += delta / count;
                m2 += delta * (observed - mean);
                var stddev = Math.Sqrt(m2 / (count - 1));
                estimate = mean + stddev;
            }

            begin = DateTime.Now;
            while ((double)(DateTime.Now - begin).Ticks / 10000d < milliseconds) ;
        }

    }
}
