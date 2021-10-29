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
        
        public MachWindow(MachOptions mo) : base(mo.WOpt.Title, mo.WOpt.X, mo.WOpt.Y, mo.WOpt.Width, mo.WOpt.Height, mo.WOpt.Flag, mo.WOpt.ThreadedProcessing)
        {

        }


    }
}
