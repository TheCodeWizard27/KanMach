﻿using System;
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
        private ShaderSetDescription _shaderSet;

        public Pipeline Pipeline { get; set; }
        public ShaderData Shader { get; set; }
        public RenderContext Context { get; set; }


        public Material()
        {
        }

        public Material(RenderContext context, ShaderData shader)
        {
            var factory = context.ResourceFactory;

            Shader = shader;

            _mvpLayout = factory.CreateResourceLayout(
                new ResourceLayoutDescription(
                        new ResourceLayoutElementDescription("ModelBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                        new ResourceLayoutElementDescription("ViewBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                        new ResourceLayoutElementDescription("ProjectionBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex)
                ));

            _shaderSet = new ShaderSetDescription(
                new[]
                {
                    new VertexLayoutDescription(
                        new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3),
                        new VertexElementDescription("Normal", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3),
                        new VertexElementDescription("UV", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2))
                },shader.Shaders);

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
                    context.ProjectionBuffer));
        }

        public virtual void Prepare(CommandList cmdList)
        {
            cmdList.SetPipeline(Pipeline);
            cmdList.SetGraphicsResourceSet(0, _mvpSet);
        }

        public virtual void Dispose()
        {
            _mvpSet.Dispose();
            _mvpLayout.Dispose();
            Pipeline.Dispose();
        }
    }
}
