using KanMach.Veldrid.Components;
using KanMach.Veldrid.Rendering.Structures;
using KanMach.Veldrid.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Veldrid;

namespace KanMach.Veldrid.EmbeddedShaders
{
    public class BasicMaterial : Material
    {

        private TextureView _diffuseTexture;

        private static ShaderData _phongShader;

        private DeviceBuffer _lightPositionBuffer;
        private DeviceBuffer _lightColorBuffer;
        private DeviceBuffer _ambientColorBuffer;
        private DeviceBuffer _materialProperties;

        private ResourceLayout _mvpLayout;
        private ResourceLayout _materialLayout;

        private ResourceSet _mvpSet;
        private ResourceSet _materialSet;
        private ShaderSetDescription _shaderSet;

        public Vector3 LightPos = Vector3.One;
        public Vector3 LightColor = Vector3.One;
        public Vector3 AmbientColor = Vector3.One;

        public MaterialProperties MaterialProperties = new MaterialProperties();

        public TextureView DiffuseTexture { 
            get => _diffuseTexture; 
            set
            {
                SetDiffuse(value);
            } 
        }

        private unsafe BasicMaterial(RenderContext context, ShaderData shader, PrimitiveTopology primitiveTopology)
        {
            Context = context;
            var factory = context.ResourceFactory;
            Shader = shader;

            // Move out to some static code.
            var empty = factory.CreateTexture(TextureDescription.Texture2D(1, 1, 1, 1, PixelFormat.B8_G8_R8_A8_UNorm, TextureUsage.Sampled));
            var emptyColor = RgbaByte.Pink;
            context.GraphicsDevice.UpdateTexture(empty, (IntPtr)(&emptyColor), 4, 0, 0, 0, 1, 1, 1, 0, 0);

            _diffuseTexture = factory.CreateTextureView(empty);

            _lightPositionBuffer = factory.CreateBuffer(new BufferDescription(16, BufferUsage.UniformBuffer));
            _lightColorBuffer = factory.CreateBuffer(new BufferDescription(16, BufferUsage.UniformBuffer));
            _ambientColorBuffer = factory.CreateBuffer(new BufferDescription(16, BufferUsage.UniformBuffer));
            _materialProperties = factory.CreateBuffer(new BufferDescription((uint) Unsafe.SizeOf<MaterialProperties>(), BufferUsage.UniformBuffer));

            _mvpLayout = factory.CreateResourceLayout(
                new ResourceLayoutDescription(
                    new ResourceLayoutElementDescription("ModelBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                    new ResourceLayoutElementDescription("ViewBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                    new ResourceLayoutElementDescription("ProjectionBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                    new ResourceLayoutElementDescription("LightPos", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                    new ResourceLayoutElementDescription("LightColor", ResourceKind.UniformBuffer, ShaderStages.Fragment),
                    new ResourceLayoutElementDescription("AmbientColor", ResourceKind.UniformBuffer, ShaderStages.Fragment)
                ));
            _materialLayout = factory.CreateResourceLayout(
                new ResourceLayoutDescription(
                    new ResourceLayoutElementDescription("MaterialProperties", ResourceKind.UniformBuffer, ShaderStages.Fragment),
                    new ResourceLayoutElementDescription("DiffuseTexture", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
                    new ResourceLayoutElementDescription("DiffuseSampler", ResourceKind.Sampler, ShaderStages.Fragment)
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
                BlendStateDescription.SingleAlphaBlend,
                DepthStencilStateDescription.DepthOnlyLessEqual,
                new RasterizerStateDescription(
                    cullMode: FaceCullMode.Back,
                    fillMode: PolygonFillMode.Solid,
                    frontFace: FrontFace.Clockwise,
                    depthClipEnabled: true,
                    scissorTestEnabled: false
                    ),
                primitiveTopology,
                _shaderSet,
                new []
                {
                    _mvpLayout,
                    _materialLayout,
                },
                context.GraphicsDevice.MainSwapchain.Framebuffer.OutputDescription));

            _mvpSet = factory.CreateResourceSet(
                new ResourceSetDescription(
                    _mvpLayout,
                    context.ModelBuffer,
                    context.ViewBuffer,
                    context.ProjectionBuffer,
                    _lightPositionBuffer,
                    _lightColorBuffer,
                    _ambientColorBuffer
                    ));

            InitMaterialSet();
        }

        public override void Prepare(CommandList cmdList)
        {
            cmdList.SetPipeline(Pipeline);
            cmdList.SetGraphicsResourceSet(0, _mvpSet);
            cmdList.SetGraphicsResourceSet(1, _materialSet);
            cmdList.UpdateBuffer(_lightPositionBuffer, 0, LightPos);
            cmdList.UpdateBuffer(_lightColorBuffer, 0, LightColor);
            cmdList.UpdateBuffer(_ambientColorBuffer, 0, AmbientColor);
            cmdList.UpdateBuffer(_materialProperties, 0, MaterialProperties);
        }

        public override void Dispose()
        {
            _mvpSet.Dispose();
            _mvpLayout.Dispose();
            Pipeline.Dispose();
        }

        public void SetDiffuse(TextureView textureView)
        {
            _diffuseTexture = textureView;
            InitMaterialSet();
        }

        // TODO move this to use renderContext and not be static.
        public static BasicMaterial NewInstance(RenderContext context, 
            PrimitiveTopology primitiveTopology = PrimitiveTopology.TriangleList)
        {
            if(_phongShader == null)
            {
                var assembly = Assembly.GetExecutingAssembly();
                var fragShader = assembly.GetEmbeddedRessource("KanMach.Veldrid.Rendering.EmbeddedShaders.Basic.frag");
                var vertShader = assembly.GetEmbeddedRessource("KanMach.Veldrid.Rendering.EmbeddedShaders.Basic.vert");
                _phongShader = new ShaderData(context, vertShader, fragShader);
            }

            return new BasicMaterial(context, _phongShader, primitiveTopology);
        }

        private void InitMaterialSet()
        {
            if (_materialSet != null)
            {
                _materialSet.Dispose();
            }

            _materialSet = Context.ResourceFactory.CreateResourceSet(
                new ResourceSetDescription(
                    _materialLayout,
                    _materialProperties,
                    DiffuseTexture,
                    Context.GraphicsDevice.Aniso4xSampler
                ));
        }

    }
}
