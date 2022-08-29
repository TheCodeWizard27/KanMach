using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veldrid;
using Veldrid.StartupUtilities;

namespace KanMach.Veldrid.Util.Options
{
    public class MachOptions
    {
        public WindowCreateInfo WindowOptions;
        public VeldridInputManagerOptions Sdl2InputManagerOptions { get; set; }
        public GraphicsDeviceOptions GraphicsDeviceOptions { get; set; }
        public GraphicsBackend Backend { get; set; }

        public MachOptions()
        {
            WindowOptions = new WindowCreateInfo()
            {
                WindowHeight = 600,
                WindowWidth = 800,
                WindowTitle = "KanMach",
                WindowInitialState = WindowState.Normal,
                X = 100,
                Y = 100,
            };

            GraphicsDeviceOptions = new GraphicsDeviceOptions(
                debug: false,
                swapchainDepthFormat: PixelFormat.R16_UNorm,
                syncToVerticalBlank: false,
                resourceBindingModel: ResourceBindingModel.Improved,
                // TODO They may not be supported greatly maybe change to use an alternative.
                preferDepthRangeZeroToOne: true,
                preferStandardClipSpaceYDirection: true
            );
            Backend = GraphicsBackend.Vulkan;

            Sdl2InputManagerOptions = new VeldridInputManagerOptions()
            {
                GamePadPollingEnabled = false
            };
        }

        public MachOptions(WindowCreateInfo wOpt, GraphicsDeviceOptions gOpt)
        {
            WindowOptions = wOpt;
            GraphicsDeviceOptions = gOpt;
        }

        public void UseGamepads(Action<VeldridInputManagerOptions> configurator = null)
        {
            Sdl2InputManagerOptions = new VeldridInputManagerOptions()
            {
                GamePadPollingEnabled = true
            };
            configurator?.Invoke(Sdl2InputManagerOptions);
        }

    }
}
