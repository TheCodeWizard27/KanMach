using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanMach.Veldrid.Util.Options
{
    public class VeldridInputManagerOptions
    {
        public int Interval { get; set; } = 1000 / 60;
        public bool GamePadPollingEnabled { get; set; }

    }
}
