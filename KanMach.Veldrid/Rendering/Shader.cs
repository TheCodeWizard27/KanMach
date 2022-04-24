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
    public class Shader
    {

        public ShaderSetDescription ShaderSet { get; protected set; }

        public Shader(RenderContext context, string vertexCode, string fragmentCode)
        {
            ShaderSet = new ShaderSetDescription(
                new[]
                {
                    new VertexLayoutDescription(
                        new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3))
                },
                context.ResourceFactory.CreateFromSpirv(
                    new ShaderDescription(ShaderStages.Vertex, Encoding.UTF8.GetBytes(vertexCode), "main"),
                    new ShaderDescription(ShaderStages.Fragment, Encoding.UTF8.GetBytes(fragmentCode), "main")));
        }

    }
}
