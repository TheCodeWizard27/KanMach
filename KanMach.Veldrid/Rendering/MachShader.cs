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
        public ShaderSetDescription ShaderSet;
        public ResourceLayout  ModelLayout;
        public ResourceLayout VertexLayout;
        public ResourceSet ModelSet;
        public ResourceSet VertexSet;

        public MachShader(ResourceFactory factory, DeviceBuffer modelBuffer, DeviceBuffer viewBuffer, DeviceBuffer projectionBuffer)
        {
            VertexCode = File.ReadAllText(@"Shaders/BaseVertexShader.vert");
            FragmentCode = File.ReadAllText(@"Shaders/BaseFragmentShader.frag");

            ShaderSet = new ShaderSetDescription(
                new[]
                {
                    new VertexLayoutDescription(
                        new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3))
                },
                factory.CreateFromSpirv(
                    new ShaderDescription(ShaderStages.Vertex, Encoding.UTF8.GetBytes(VertexCode), "main"),
                    new ShaderDescription(ShaderStages.Fragment, Encoding.UTF8.GetBytes(FragmentCode), "main")));

            ModelLayout = factory.CreateResourceLayout(
                new ResourceLayoutDescription(
                        new ResourceLayoutElementDescription("ModelBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex)));

            VertexLayout = factory.CreateResourceLayout(
                new ResourceLayoutDescription(
                        new ResourceLayoutElementDescription("ViewBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                        new ResourceLayoutElementDescription("ProjectionBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex)
                        ));

            ModelSet = factory.CreateResourceSet(
                new ResourceSetDescription(
                    ModelLayout,
                    modelBuffer));

            VertexSet = factory.CreateResourceSet(
                new ResourceSetDescription(
                    VertexLayout,
                    viewBuffer,
                    projectionBuffer
                    ));
        }
    }
}
