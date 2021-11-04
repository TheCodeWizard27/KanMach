using KanMach.Veldrid.Util.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanMach.Veldrid.Util
{
    public interface IVeldridService
    {
        void DisposeResources();
        void ConfigureVeldrid();
        void Draw();
        void CreateResources();
        void InitService(MachOptions mo);
    }
}
