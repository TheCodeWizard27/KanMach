using KanMach.Veldrid.Components;
using KanMach.Veldrid.Graphics;
using KanMach.Veldrid.Graphics.Cameras;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Veldrid;

namespace KanMach.Veldrid.Rendering.Renderers
{
    public class ForwardSceneRenderer
    {
        private RenderContext _renderContext { get => _sceneContext.RenderContext; }
        private SceneRenderContext _sceneContext;

        public Camera Camera { get; set; }
        public Vector2 ViewPort { get; private set; }

        public ForwardSceneRenderer(SceneRenderContext sceneContext)
        {
            _sceneContext = sceneContext;

            ViewPort = new Vector2(
                _renderContext.GraphicsDevice.SwapchainFramebuffer.Width, 
                _renderContext.GraphicsDevice.SwapchainFramebuffer.Height);

            Camera = new SimpleCamera(_renderContext, ViewPort);
        }

        public void Draw(IEnumerable<(MeshRenderer, Transform)> meshRenderers)
        {
            var commandList = _renderContext.BeginDraw();
            Draw(meshRenderers, commandList);
            _renderContext.EndDraw();
        }

        public void Draw(IEnumerable<(MeshRenderer Renderer, Transform Transform)> meshRenderers, CommandList commandList)
        {
            commandList.UpdateBuffer(_renderContext.ViewBuffer, 0, Camera.GetView());
            commandList.UpdateBuffer(_renderContext.ProjectionBuffer, 0, Camera.GetPerspective());

            foreach (var model in meshRenderers)
            {
                // TODO how to get transformation in 'ere?
                var modelMatrix =
                    Matrix4x4.CreateScale(model.Transform.Scale) * Matrix4x4.CreateTranslation(model.Transform.Position)
                    * Matrix4x4.CreateRotationX(model.Transform.Rotation.X)
                    * Matrix4x4.CreateRotationY(model.Transform.Rotation.Y)
                    * Matrix4x4.CreateRotationZ(model.Transform.Rotation.Z);

                commandList.UpdateBuffer(_renderContext.ModelBuffer, 0, ref modelMatrix);

                model.Renderer.Render(commandList);
            }
        }

    }
}
