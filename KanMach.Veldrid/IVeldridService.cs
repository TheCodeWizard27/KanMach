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

        bool KeepMouseCentered { get; set; }
        bool MouseVisible { get; set; }

        InputSnapshot CurrentInputSnapshot { get; }

        delegate void OnInitHandler();
        event OnInitHandler OnInit;

        delegate void OnCloseHandler();
        event OnCloseHandler OnClose;

        delegate void OnUpdateHandler(TimeSpan delta);
        event OnUpdateHandler OnUpdate;

        void Update(TimeSpan delta);
        void Close();
        void DisposeResources();
        void Init();

    }
}
