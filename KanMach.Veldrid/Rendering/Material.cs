using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veldrid;

namespace KanMach.Veldrid.Components
{
    public class Material : IDisposable
    {

        private ResourceLayout _mvpLayout;
        private ResourceSet _mvpSet;

        public Pipeline Pipeline { get; set; }
        public Shader Shader { get; set; }

        public Material(RenderContext context, Shader shader)
        {
            var factory = context.ResourceFactory;

            Shader = shader;

            _mvpLayout = factory.CreateResourceLayout(
                new ResourceLayoutDescription(
                        new ResourceLayoutElementDescription("ModelBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                        new ResourceLayoutElementDescription("ViewBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                        new ResourceLayoutElementDescription("ProjectionBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex)
                ));

            Pipeline = factory.CreateGraphicsPipeline(new GraphicsPipelineDescription(
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
                shader.ShaderSet,
                _mvpLayout,
                context.GraphicsDevice.MainSwapchain.Framebuffer.OutputDescription));

            _mvpSet = factory.CreateResourceSet(
                new ResourceSetDescription(
                    _mvpLayout,
                    context.ModelBuffer,
                    context.ViewBuffer,
                    context.ProjectionBuffer));
        }

        public void Prepare(CommandList cmdList)
        {
            cmdList.SetPipeline(Pipeline);
            cmdList.SetGraphicsResourceSet(0, _mvpSet);
        }

        public void Dispose()
        {
            _mvpSet.Dispose();
            _mvpLayout.Dispose();
            Pipeline.Dispose();
        }
    }
}
