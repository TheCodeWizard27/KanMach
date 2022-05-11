using KanMach.Veldrid.Util.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;

namespace KanMach.Veldrid.Graphics
{
    public class MachWindow : Sdl2Window
    {
        
        public MachWindow(MachOptions mo) : base(mo.WindowOptions.Title, mo.WindowOptions.X, mo.WindowOptions.Y, mo.WindowOptions.Width, mo.WindowOptions.Height, mo.WindowOptions.Flag, mo.WindowOptions.ThreadedProcessing)
        {




        }


    }
}
