using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanMach.Veldrid.Input
{
    public interface IVeldridInputManager
    {

        Keyboard Keyboard { get; }
        Mouse Mouse { get; }
        Gamepads Gamepads { get; }

    }
}
