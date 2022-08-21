using KanMach.Veldrid.Components;
using KanMach.Veldrid.Util.Options;
using System;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;
using static KanMach.Veldrid.IVeldridService;

namespace KanMach.Veldrid
{
    public class VeldridService : IVeldridService
    {
        
        private MachOptions _machOptions;
        
        public Sdl2Window MachWindow { get; private set; }
        public InputSnapshot CurrentInputSnapshot { get; private set; }
        public RenderContext RenderContext { get; private set; }

        public event OnCloseHandler OnClose;

        public VeldridService(MachOptions machOptions)
        {
            _machOptions = machOptions ?? new MachOptions();
        }

        public void Init()
        {
            MachWindow = VeldridStartup.CreateWindow(_machOptions.WindowOptions);
            MachWindow.Closed += () => OnClose?.Invoke();

            var graphicsDevice = VeldridStartup.CreateGraphicsDevice(MachWindow, _machOptions.GraphicsDeviceOptions, _machOptions.Backend);
            RenderContext = new RenderContext(graphicsDevice);
        }

        public void Update(TimeSpan delta)
        {
            CurrentInputSnapshot = MachWindow.PumpEvents();
        }

        public void DisposeResources()
        {
            RenderContext.GraphicsDevice.Dispose();
        }

        public void Close()
        {
            MachWindow.Close();
        }

    }
}
