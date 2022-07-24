using ImGuiNET;
using KanMach.Veldrid.Components;
using KanMach.Veldrid.EmbeddedShaders;
using KanMach.Veldrid.Graphics;
using KanMach.Veldrid.Model;
using KanMach.Veldrid.Model.Src;
using KanMach.Veldrid.Util;
using KanMach.Veldrid.Util.Options;
using System;
using System.Numerics;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;
using static KanMach.Veldrid.IVeldridService;

namespace KanMach.Veldrid
{
    public class VeldridService : IVeldridService
    {
        
        private MachOptions _machOptions;

        public MachWindow MachWindow { get; private set; }
        public RenderContext RenderContext { get; private set; }

        public event OnCloseHandler OnClose;

        public VeldridService(MachOptions machOptions)
        {
            _machOptions = machOptions ?? new MachOptions();
        }

        public void Init()
        {
            MachWindow = new MachWindow(_machOptions);
            MachWindow.Closed += () => OnClose?.Invoke();

            var graphicsDevice = VeldridStartup.CreateGraphicsDevice(MachWindow,  _machOptions.GraphicsDeviceOptions);

            RenderContext = new RenderContext(graphicsDevice);
        }

        public void DisposeResources()
        {
            RenderContext.GraphicsDevice.Dispose();
        }

        public void Close()
        {
            MachWindow.Close();
        }

        public void PumpEvents()
        {
            MachWindow.PumpEvents();
        }
    }
}
