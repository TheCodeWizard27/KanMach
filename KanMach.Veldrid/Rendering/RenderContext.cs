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
        protected CommandList _commandList;

        public Dictionary<string, DeviceBuffer> SharedBuffers { get; private set; }

        public DeviceBuffer ModelBuffer { get; private set; }
        public DeviceBuffer ViewBuffer { get; private set; }
        public DeviceBuffer ProjectionBuffer { get; private set; }

        public GraphicsDevice GraphicsDevice { get; set; }
        public ResourceFactory ResourceFactory { get => GraphicsDevice.ResourceFactory; }

        public RenderContext(GraphicsDevice graphicsDevice)
        {
            SharedBuffers = new Dictionary<string, DeviceBuffer>();

            GraphicsDevice = graphicsDevice;

            ModelBuffer = ResourceFactory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer));
            ViewBuffer = ResourceFactory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer));
            ProjectionBuffer = ResourceFactory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer));

            _commandList = ResourceFactory.CreateCommandList();
        }

        public virtual CommandList BeginDraw()
        {
            _commandList.Begin();

            _commandList.SetFramebuffer(GraphicsDevice.MainSwapchain.Framebuffer);

            _commandList.ClearColorTarget(0, RgbaFloat.Black);
            _commandList.ClearDepthStencil(1f);

            return _commandList;
        }

        public virtual void EndDraw()
        {
            _commandList.End();
            GraphicsDevice.SubmitCommands(_commandList);
            GraphicsDevice.SwapBuffers(GraphicsDevice.MainSwapchain);
            GraphicsDevice.WaitForIdle();
        }

    }
}
