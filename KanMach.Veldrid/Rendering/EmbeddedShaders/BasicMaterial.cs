using KanMach.Veldrid.Components;
using KanMach.Veldrid.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Veldrid;

namespace KanMach.Veldrid.EmbeddedShaders
{
    public class BasicMaterial : Material
    {
        private DeviceBuffer _colorBuffer;

        private ResourceLayout _mvpLayout;
        private ResourceSet _mvpSet;
        private ShaderSetDescription _shaderSet;

        private static ShaderData _basicShader;

        public Vector3 Color { get; set; } = Vector3.One;

        private BasicMaterial(RenderContext context, ShaderData shader)
        {
            var factory = context.ResourceFactory;

            Shader = shader;

            _colorBuffer = factory.CreateBuffer(new BufferDescription(16, BufferUsage.UniformBuffer));

            _mvpLayout = factory.CreateResourceLayout(
                new ResourceLayoutDescription(
                        new ResourceLayoutElementDescription("ModelBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                        new ResourceLayoutElementDescription("ViewBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                        new ResourceLayoutElementDescription("ProjectionBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                        new ResourceLayoutElementDescription("Color", ResourceKind.UniformBuffer, ShaderStages.Fragment)
                ));

            _shaderSet = new ShaderSetDescription(
                new[]
                {
                    new VertexLayoutDescription(
                        new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3),
                        new VertexElementDescription("Normal", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3),
                        new VertexElementDescription("UV", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2))
                }, shader.Shaders);

            Pipeline = factory.CreateGraphicsPipeline(new GraphicsPipelineDescription(
                BlendStateDescription.SingleOverrideBlend,
                new DepthStencilStateDescription(
                    depthTestEnabled: true,
                    depthWriteEnabled: true,
                    comparisonKind: ComparisonKind.LessEqual
                    ),
                new RasterizerStateDescription(
                    cullMode: FaceCullMode.Back,
                    fillMode: PolygonFillMode.Solid,
                    frontFace: FrontFace.Clockwise,
                    depthClipEnabled: true,
                    scissorTestEnabled: false
                    ),
                PrimitiveTopology.TriangleList,
                _shaderSet,
                _mvpLayout,
                context.GraphicsDevice.MainSwapchain.Framebuffer.OutputDescription));

            _mvpSet = factory.CreateResourceSet(
                new ResourceSetDescription(
                    _mvpLayout,
                    context.ModelBuffer,
                    context.ViewBuffer,
                    context.ProjectionBuffer,
                    _colorBuffer));
        }

        public override void Prepare(CommandList cmdList)
        {
            cmdList.SetPipeline(Pipeline);
            cmdList.SetGraphicsResourceSet(0, _mvpSet);
            cmdList.UpdateBuffer(_colorBuffer, 0, Color);
        }

        public override void Dispose()
        {
            _mvpSet.Dispose();
            _mvpLayout.Dispose();
            Pipeline.Dispose();
        }

        public static BasicMaterial GetInstance(RenderContext context)
        {
            if(_basicShader == null)
            {
                var assembly = Assembly.GetExecutingAssembly();
                var fragShader = assembly.GetEmbeddedRessource("KanMach.Veldrid.Rendering.EmbeddedShaders.Basic.frag");
                var vertShader = assembly.GetEmbeddedRessource("KanMach.Veldrid.Rendering.EmbeddedShaders.Basic.vert");
                _basicShader = new ShaderData(context, vertShader, fragShader);
            }

            return new BasicMaterial(context, _basicShader);
        }

    }
}
