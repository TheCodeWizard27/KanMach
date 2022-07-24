using KanMach.Veldrid.Components;
using KanMach.Veldrid.Util.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veldrid;

namespace KanMach.Veldrid
{
    public interface IVeldridService
    {

        RenderContext RenderContext { get; }

        delegate void OnCloseHandler();
        event OnCloseHandler OnClose;

        void Close();
        void DisposeResources();
        void Init();

        void PumpEvents();
    }
}
