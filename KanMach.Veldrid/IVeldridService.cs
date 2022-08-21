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

        InputSnapshot CurrentInputSnapshot { get; }

        delegate void OnCloseHandler();
        event OnCloseHandler OnClose;

        void Update(TimeSpan delta);
        void Close();
        void DisposeResources();
        void Init();

    }
}
