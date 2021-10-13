using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veldrid;
using Veldrid.SPIRV;

namespace KanMach.Veldrid.Components
{
    public class MachShader
    {
        private string VertexCode;
        private string FragmentCode;
        public ShaderSetDescription shaderSet;
        public ResourceLayout modelLayout;
        public ResourceLayout vertexLayout;
        public ResourceSet modelSet;
        public ResourceSet vertexSet;

        public MachShader(ResourceFactory factory, DeviceBuffer modelBuffer, DeviceBuffer viewBuffer, DeviceBuffer projectionBuffer)
        {
            VertexCode = File.ReadAllText(@"Shaders/BaseVertexShader.vert");
            FragmentCode = File.ReadAllText(@"Shaders/BaseFragmentShader.frag");

            shaderSet = new ShaderSetDescription(
                new[]
                {
                    new VertexLayoutDescription(
                        new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3))
                },
                factory.CreateFromSpirv(
                    new ShaderDescription(ShaderStages.Vertex, Encoding.UTF8.GetBytes(VertexCode), "main"),
                    new ShaderDescription(ShaderStages.Fragment, Encoding.UTF8.GetBytes(FragmentCode), "main")));

            modelLayout = factory.CreateResourceLayout(
                new ResourceLayoutDescription(
                        new ResourceLayoutElementDescription("ModelBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex)));

            vertexLayout = factory.CreateResourceLayout(
                new ResourceLayoutDescription(
                        new ResourceLayoutElementDescription("ViewBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                        new ResourceLayoutElementDescription("ProjectionBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex)
                        ));

            modelSet = factory.CreateResourceSet(
                new ResourceSetDescription(
                    modelLayout,
                    modelBuffer));

            vertexSet = factory.CreateResourceSet(
                new ResourceSetDescription(
                    vertexLayout,
                    viewBuffer,
                    projectionBuffer
                    ));
        }
    }
}
