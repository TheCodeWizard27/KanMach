using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veldrid;

namespace KanMach.Veldrid.Components
{
    public class SceneRenderContext
    {

        public RenderContext RenderContext { get; set; }

        public SceneRenderContext(RenderContext renderContext)
        {

            RenderContext = renderContext;

        }

    }
}
