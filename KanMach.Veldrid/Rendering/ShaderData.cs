using System.Text;
using Veldrid;
using Veldrid.SPIRV;

namespace KanMach.Veldrid.Components
{
    public class ShaderData
    {

        public Shader[] Shaders { get; protected set; }

        public ShaderData(RenderContext context, string vertexCode, string fragmentCode)
        {
            Shaders = context.ResourceFactory.CreateFromSpirv(
                    new ShaderDescription(ShaderStages.Vertex, Encoding.UTF8.GetBytes(vertexCode), "main"),
                    new ShaderDescription(ShaderStages.Fragment, Encoding.UTF8.GetBytes(fragmentCode), "main"));
            

        }
    }
}
