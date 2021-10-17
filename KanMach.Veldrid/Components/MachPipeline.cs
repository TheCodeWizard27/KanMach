using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veldrid;

namespace KanMach.Veldrid.Components
{
    class MachPipeline
    {
        public Pipeline Pipeline;

        public MachPipeline(GraphicsDevice graphicsDevice, MachShader shader)
        {
            Pipeline = graphicsDevice.ResourceFactory.CreateGraphicsPipeline(new GraphicsPipelineDescription(
                BlendStateDescription.SingleOverrideBlend,
                new DepthStencilStateDescription(
                    depthTestEnabled: true,
                    depthWriteEnabled: true,
                    comparisonKind: ComparisonKind.Always
                    ),
                new RasterizerStateDescription(
                    cullMode: FaceCullMode.Back,
                    fillMode: PolygonFillMode.Solid,
                    frontFace: FrontFace.Clockwise,
                    depthClipEnabled: false,
                    scissorTestEnabled: false
                    ),
                PrimitiveTopology.TriangleList,
                shader.shaderSet,
                new[] { shader.modelLayout, shader.vertexLayout },
                graphicsDevice.MainSwapchain.Framebuffer.OutputDescription));
        }
    }
}
