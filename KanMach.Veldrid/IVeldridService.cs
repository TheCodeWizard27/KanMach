using KanMach.Veldrid.Util.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanMach.Veldrid
{
    public interface IVeldridService
    {
        delegate void OnCloseHandler();
        event OnCloseHandler OnClose;

        void Close();
        void DisposeResources();
        void CreateResources();
        void Init();

        void Draw();
        void PumpEvents();
    }
}
