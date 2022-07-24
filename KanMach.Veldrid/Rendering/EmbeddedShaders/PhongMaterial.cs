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
    public class PhongMaterial : Material
    {

        private static ShaderData _phongShader;

        private PhongMaterial(RenderContext context, ShaderData shader) : base(context, shader)
        {

        }

        public static Material GetInstance(RenderContext context)
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
