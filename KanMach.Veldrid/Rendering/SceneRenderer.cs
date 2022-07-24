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
        public Vector2 ViewPort { get; private set; }

        public SceneRenderer(RenderContext context)
        {
            _context = context;

            ViewPort = new Vector2(
                context.GraphicsDevice.SwapchainFramebuffer.Width, 
                context.GraphicsDevice.SwapchainFramebuffer.Height);

            Camera = new Camera(ViewPort);
        }

        public void Draw(IEnumerable<(MeshRenderer, Transform)> meshRenderers)
        {
            var commandList = _context.BeginDraw();

            commandList.UpdateBuffer(_context.ViewBuffer, 0, Camera.GetView());
            commandList.UpdateBuffer(_context.ProjectionBuffer, 0, Camera.GetPerspective());

            foreach(var model in meshRenderers)
            {
                // TODO how to get transformation in 'ere?
                var modelMatrix =
                    Matrix4x4.CreateTranslation(model.Item2.Position)
                    * Matrix4x4.CreateRotationX(model.Item2.Rotation.X)
                    * Matrix4x4.CreateRotationY(model.Item2.Rotation.Y)
                    * Matrix4x4.CreateRotationZ(model.Item2.Rotation.Z)
                    * Matrix4x4.CreateScale(model.Item2.Scale);

                commandList.UpdateBuffer(_context.ModelBuffer, 0, ref modelMatrix);

                model.Item1.Render(commandList);
            }

            _context.EndDraw();
        }

    }
}
