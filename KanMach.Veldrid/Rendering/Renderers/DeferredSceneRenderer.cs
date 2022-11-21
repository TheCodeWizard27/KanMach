using KanMach.Veldrid.Components;
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
    public class DeferredSceneRenderer
    {
        private RenderContext _renderContext { get => _sceneContext.RenderContext; }
        private SceneRenderContext _sceneContext;

        private CommandList _commandList;

        private Framebuffer _gBuffer;
        private Texture _depthTexture;
        private Texture _positionTexture;
        private Texture _normalTexture;
        private Texture _colorTexture;

        public Camera Camera { get; set; }
        public Vector2 ViewPort { get; private set; }

        public DeferredSceneRenderer(SceneRenderContext sceneContext)
        {
            _sceneContext = sceneContext;

            ViewPort = new Vector2(
                _renderContext.GraphicsDevice.SwapchainFramebuffer.Width,
                _renderContext.GraphicsDevice.SwapchainFramebuffer.Height);

            Camera = new SimpleCamera(_renderContext, ViewPort);

            _commandList = _renderContext.ResourceFactory.CreateCommandList();

            #region init G Buffer

            _depthTexture = _renderContext.ResourceFactory.CreateTexture(TextureDescription.Texture2D(
                (uint) ViewPort.X, 
                (uint) ViewPort.Y, 
                mipLevels: 1, 
                arrayLayers: 1, 
                PixelFormat.R32_Float, 
                TextureUsage.DepthStencil, 
                TextureSampleCount.Count1));

            _positionTexture = _renderContext.ResourceFactory.CreateTexture(TextureDescription.Texture2D(
                (uint)ViewPort.X,
                (uint)ViewPort.Y,
                mipLevels: 1,
                arrayLayers: 1,
                PixelFormat.R32_G32_B32_A32_Float,
                TextureUsage.RenderTarget,
                TextureSampleCount.Count1));
            _normalTexture = _renderContext.ResourceFactory.CreateTexture(TextureDescription.Texture2D(
                (uint)ViewPort.X,
                (uint)ViewPort.Y,
                mipLevels: 1,
                arrayLayers: 1,
                PixelFormat.R32_G32_B32_A32_Float,
                TextureUsage.RenderTarget,
                TextureSampleCount.Count1));
            _colorTexture = _renderContext.ResourceFactory.CreateTexture(TextureDescription.Texture2D(
                (uint)ViewPort.X,
                (uint)ViewPort.Y,
                mipLevels: 1,
                arrayLayers: 1,
                PixelFormat.R32_G32_B32_A32_UInt,
                TextureUsage.RenderTarget,
                TextureSampleCount.Count1));

            _gBuffer = _renderContext.ResourceFactory.CreateFramebuffer(new FramebufferDescription(
                _depthTexture,
                _positionTexture,
                _normalTexture,
                _colorTexture));

            #endregion

        }

        public void Draw(IEnumerable<(MeshRenderer, Transform)> meshRenderers)
        {
            _commandList.Begin();


            #region draw g buffer

            _commandList.PushDebugGroup("G Buffer");

            _commandList.SetFramebuffer(_gBuffer);
            
            _commandList.ClearDepthStencil(1f);

            _commandList.ClearColorTarget(0, RgbaFloat.Black);
            _commandList.ClearColorTarget(1, RgbaFloat.Black);
            _commandList.ClearColorTarget(2, RgbaFloat.Black);

            Draw(meshRenderers, _commandList);

            _commandList.PopDebugGroup();

            _commandList.End();

            _renderContext.GraphicsDevice.SubmitCommands(_commandList);

            #endregion

            // TODO implement actual rendering.
            var commandList = _renderContext.BeginDraw();



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
