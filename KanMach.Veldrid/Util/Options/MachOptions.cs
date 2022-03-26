using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veldrid;

namespace KanMach.Veldrid.Util.Options
{
    public class MachOptions
    {
        public MachWindowOptions WindowOptions { get; set; }
        public Sdl2InputManagerOptions Sdl2InputManagerOptions { get; set; }
        public GraphicsDeviceOptions GraphicsDeviceOptions { get; set; }

        public MachOptions()
        {
            WindowOptions = new MachWindowOptions();

            GraphicsDeviceOptions = new GraphicsDeviceOptions(
                debug: false,
                swapchainDepthFormat: PixelFormat.R16_UNorm,
                syncToVerticalBlank: false,
                resourceBindingModel: ResourceBindingModel.Improved,
                preferDepthRangeZeroToOne: true,
                preferStandardClipSpaceYDirection: false
            );

            Sdl2InputManagerOptions = new Sdl2InputManagerOptions()
            {
                GamePadPollingEnabled = false
            };
        }

        public MachOptions(MachWindowOptions wOpt, GraphicsDeviceOptions gOpt)
        {
            WindowOptions = wOpt;
            GraphicsDeviceOptions = gOpt;
        }

        public void UseGamepads(Action<Sdl2InputManagerOptions> configurator = null)
        {
            Sdl2InputManagerOptions = new Sdl2InputManagerOptions()
            {
                GamePadPollingEnabled = true
            };
            configurator?.Invoke(Sdl2InputManagerOptions);
        }

    }
}
