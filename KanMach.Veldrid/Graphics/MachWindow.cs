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
    public class MachWindow
    {
        public Sdl2Window window;
        
        public MachWindow(int x, int y, int width, int height, string title)
        {
            WindowCreateInfo windowCI = new WindowCreateInfo()
            {
                X = x,
                Y = y,
                WindowWidth = width,
                WindowHeight = height,
                WindowTitle = title
            };
            window = VeldridStartup.CreateWindow(ref windowCI);



        }


    }
}
