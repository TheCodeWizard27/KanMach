using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veldrid;

namespace KanMach.Veldrid.Components
{
    public class Material
    {

        private ResourceLayout _modelLayout;
        private ResourceLayout _vertexLayout;
        private ResourceSet _modelSet;
        private ResourceSet _vertexSet;

        public Pipeline Pipeline { get; set; }
        public Shader Shader { get; set; }

        public Material(RenderContext context, Shader shader)
        {
            var factory = context.ResourceFactory;

            Shader = shader;

            _modelLayout = factory.CreateResourceLayout(
                new ResourceLayoutDescription(
                        new ResourceLayoutElementDescription("ModelBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex)));

            _vertexLayout = factory.CreateResourceLayout(
                new ResourceLayoutDescription(
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
                new[] { _modelLayout, _vertexLayout },
                context.GraphicsDevice.MainSwapchain.Framebuffer.OutputDescription));

            _modelSet = factory.CreateResourceSet(
                new ResourceSetDescription(
                    _modelLayout,
                    context.ModelBuffer));

            _vertexSet = factory.CreateResourceSet(
                new ResourceSetDescription(
                    _vertexLayout,
                    context.ViewBuffer,
                    context.ProjectionBuffer
                    ));
        }

        public void Prepare(CommandList cmdList)
        {
            cmdList.SetPipeline(Pipeline);
            cmdList.SetGraphicsResourceSet(0, _modelSet);
            cmdList.SetGraphicsResourceSet(1, _vertexSet);
        }

    }
}
