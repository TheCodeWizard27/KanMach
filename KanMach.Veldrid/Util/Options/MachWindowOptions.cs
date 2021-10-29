using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veldrid.Sdl2;

namespace KanMach.Veldrid.Util.Options
{
    public class MachWindowOptions
    {
        public string Title { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public SDL_WindowFlags Flag { get; set; }
        public bool ThreadedProcessing { get; set; }


        public MachWindowOptions()
        {
            Title = "KanMach";
            X = 100;
            Y = 100;
            Width = 960;
            Height = 560;
            Flag = SDL_WindowFlags.Resizable;
            ThreadedProcessing = true;
        }

        public MachWindowOptions(string title, int x, int y, int width, int height, SDL_WindowFlags flag, bool threadedProcessing)
        {
            Title = title;
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Flag = flag;
            ThreadedProcessing = threadedProcessing;
        }

    }
}
