using KanMach.Veldrid.Components;
using KanMach.Veldrid.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KanMach.Veldrid.EmbeddedShaders
{
    public class BasicMaterial : Material
    {

        private static Shader _basicShader;

        private BasicMaterial(RenderContext context, Shader shader) : base(context, shader)
        {

        }

        public static Material GetInstance(RenderContext context)
        {
            if(_basicShader == null)
            {
                var assembly = Assembly.GetExecutingAssembly();
                var fragShader = assembly.GetEmbeddedRessource("KanMach.Veldrid.EmbeddedShaders.BasicShader.frag");
                var vertShader = assembly.GetEmbeddedRessource("KanMach.Veldrid.EmbeddedShaders.BasicShader.vert");
                _basicShader = new Shader(context, vertShader, fragShader);
            }

            return new BasicMaterial(context, _basicShader);
        }

    }
}
