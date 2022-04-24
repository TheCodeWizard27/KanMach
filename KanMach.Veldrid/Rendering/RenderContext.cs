using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veldrid;

namespace KanMach.Veldrid.Components
{
    public class RenderContext
    {

        public DeviceBuffer ModelBuffer { get; private set; }
        public DeviceBuffer ViewBuffer { get; private set; }
        public DeviceBuffer ProjectionBuffer { get; private set; }

        public GraphicsDevice GraphicsDevice { get; set; }
        
        public ResourceFactory ResourceFactory { get => GraphicsDevice.ResourceFactory; }

        public RenderContext(GraphicsDevice graphicsDevice)
        {
            GraphicsDevice = graphicsDevice;

            ModelBuffer = ResourceFactory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer));
            ViewBuffer = ResourceFactory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer));
            ProjectionBuffer = ResourceFactory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer));
        }

    }
}
