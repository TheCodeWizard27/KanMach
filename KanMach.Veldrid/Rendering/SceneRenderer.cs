using KanMach.Veldrid.Components;
using KanMach.Veldrid.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace KanMach.Veldrid.Rendering
{
    public class SceneRenderer
    {
        private RenderContext _context;

        public Camera Camera { get; set; }

        public SceneRenderer(RenderContext context)
        {
            _context = context;

            var viewport = new Vector2(
                context.GraphicsDevice.SwapchainFramebuffer.Width, 
                context.GraphicsDevice.SwapchainFramebuffer.Height);

            Camera = new Camera(viewport);
        }

        public void Draw(IEnumerable<MeshRenderer> meshRenderers)
        {
            var commandList = _context.BeginDraw();

            commandList.UpdateBuffer(_context.ViewBuffer, 0, Camera.GetView());
            commandList.UpdateBuffer(_context.ProjectionBuffer, 0, Camera.GetPerspective());

            meshRenderers.ForEach(renderer => renderer.Render(commandList));

            _context.EndDraw();
        }

    }
}
