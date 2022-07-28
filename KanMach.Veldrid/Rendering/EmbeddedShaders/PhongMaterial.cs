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
    public class PhongMaterial : Material
    {

        private static ShaderData _phongShader;

        private DeviceBuffer _lightPositionBuffer;
        private DeviceBuffer _lightColorBuffer;

        private ResourceLayout _mvpLayout;
        private ResourceSet _mvpSet;
        private ShaderSetDescription _shaderSet;

        public Vector3 LightPos { get; set; } = Vector3.One;
        public Vector3 LightColor { get; set; } = Vector3.One;

        private PhongMaterial(RenderContext context, ShaderData shader)
        {
            var factory = context.ResourceFactory;

            Shader = shader;

            _lightPositionBuffer = factory.CreateBuffer(new BufferDescription(16, BufferUsage.UniformBuffer));
            _lightColorBuffer = factory.CreateBuffer(new BufferDescription(16, BufferUsage.UniformBuffer));

            _mvpLayout = factory.CreateResourceLayout(
                new ResourceLayoutDescription(
                        new ResourceLayoutElementDescription("ModelBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                        new ResourceLayoutElementDescription("ViewBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                        new ResourceLayoutElementDescription("ProjectionBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                        new ResourceLayoutElementDescription("LightPos", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                        new ResourceLayoutElementDescription("LightColor", ResourceKind.UniformBuffer, ShaderStages.Fragment)
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
                    _lightPositionBuffer,
                    _lightColorBuffer
                    ));
        }

        public override void Prepare(CommandList cmdList)
        {
            cmdList.SetPipeline(Pipeline);
            cmdList.SetGraphicsResourceSet(0, _mvpSet);
            cmdList.UpdateBuffer(_lightPositionBuffer, 0, LightPos);
            cmdList.UpdateBuffer(_lightColorBuffer, 0, LightColor);
        }

        public override void Dispose()
        {
            _mvpSet.Dispose();
            _mvpLayout.Dispose();
            Pipeline.Dispose();
        }

        public static PhongMaterial NewInstance(RenderContext context)
        {
            if(_phongShader == null)
            {
                var assembly = Assembly.GetExecutingAssembly();
                var fragShader = assembly.GetEmbeddedRessource("KanMach.Veldrid.Rendering.EmbeddedShaders.Phong.frag");
                var vertShader = assembly.GetEmbeddedRessource("KanMach.Veldrid.Rendering.EmbeddedShaders.Phong.vert");
                _phongShader = new ShaderData(context, vertShader, fragShader);
            }

            return new PhongMaterial(context, _phongShader);
        }

    }
}
