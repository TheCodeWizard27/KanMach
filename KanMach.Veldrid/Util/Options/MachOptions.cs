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
        public MachWindowOptions WOpt { get; set; }
        public GraphicsDeviceOptions GDOpt { get; set; }

        public MachOptions()
        {
            WOpt = new MachWindowOptions();

            GDOpt = new GraphicsDeviceOptions(
                debug: false,
                swapchainDepthFormat: PixelFormat.R16_UNorm,
                syncToVerticalBlank: false,
                resourceBindingModel: ResourceBindingModel.Improved,
                preferDepthRangeZeroToOne: true,
                preferStandardClipSpaceYDirection: false
            );
        }

        public MachOptions(MachWindowOptions wOpt, GraphicsDeviceOptions gOpt)
        {
            WOpt = wOpt;
            GDOpt = gOpt;
        }
    }
}
