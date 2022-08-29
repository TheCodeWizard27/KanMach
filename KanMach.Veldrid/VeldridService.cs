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
        
        public bool KeepMouseCentered { get; set; }
        public bool MouseVisible { get; set; } = true;

        public Sdl2Window MachWindow { get; private set; }
        public InputSnapshot CurrentInputSnapshot { get; private set; }
        public RenderContext RenderContext { get; private set; }

        public event OnInitHandler OnInit;
        public event OnCloseHandler OnClose;
        public event OnUpdateHandler OnUpdate;

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

            OnInit?.Invoke();
        }

        public void Update(TimeSpan delta)
        {
            CurrentInputSnapshot = MachWindow.PumpEvents();
            OnUpdate?.Invoke(delta);

            MachWindow.CursorVisible = MouseVisible;

            if(KeepMouseCentered && MachWindow.Focused)
            {
                MachWindow.SetMousePosition(MachWindow.Width/2, MachWindow.Height/2);
            }
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
