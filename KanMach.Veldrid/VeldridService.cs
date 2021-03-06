using ImGuiNET;
using KanMach.Veldrid.Components;
using KanMach.Veldrid.EmbeddedShaders;
using KanMach.Veldrid.Graphics;
using KanMach.Veldrid.Model;
using KanMach.Veldrid.Model.Src;
using KanMach.Veldrid.Util;
using KanMach.Veldrid.Util.Options;
using System;
using System.Numerics;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;
using static KanMach.Veldrid.IVeldridService;

namespace KanMach.Veldrid
{
    public class VeldridService : IVeldridService
    {
        public MachWindow _machWindow;
        private MachOptions _machOptions;

        private ImGuiRenderer _imGuiRenderer;
        private CommandList _cl; 
        
        private float _tick;

        public RenderContext RenderContext { get; set; }

        public event OnCloseHandler OnClose;

        public VeldridService(MachOptions machOptions)
        {
            _machOptions = machOptions ?? new MachOptions();
        }

        public void Init()
        {
            _machWindow = new MachWindow(_machOptions);
            _machWindow.Closed += () => OnClose?.Invoke();

            var graphicsDevice = VeldridStartup.CreateGraphicsDevice(_machWindow,  _machOptions.GraphicsDeviceOptions);
            _imGuiRenderer = new ImGuiRenderer(
                graphicsDevice,
                graphicsDevice.MainSwapchain.Framebuffer.OutputDescription,
                _machWindow.Width,
                _machWindow.Height);

            _machWindow.Resized += _machWindow_Resized;
            RenderContext = new RenderContext(graphicsDevice);

            CreateResources();
        }

        private void _machWindow_Resized()
        {
            _imGuiRenderer.WindowResized(_machWindow.Width, _machWindow.Height);
            //_renderContext.GraphicsDevice.MainSwapchain.Resize((uint)_machWindow.Width, (uint)_machWindow.Height);
        }

        public void CreateResources()
        {
            ResourceFactory factory = RenderContext.ResourceFactory;

            _cl = factory.CreateCommandList();
        }

        public void Draw()
        {
            var snapshot = _machWindow.PumpEvents();
            _imGuiRenderer.Update(1f / 60f, snapshot);

            if (ImGui.Begin("Test Window"))
            {
                if (ImGui.Button("Quit"))
                {
                    _machWindow.Close();
                }
            }

            ImGui.End();

            _cl.Begin();
            _tick += 0.0001f;
            Matrix4x4 modelMatrix =
                Matrix4x4.CreateTranslation(0f, 0, -0.01f)
                * Matrix4x4.CreateRotationX(0f)
                * Matrix4x4.CreateRotationY(0f)
                * Matrix4x4.CreateRotationZ(_tick)
                * Matrix4x4.CreateScale(1.0f);

            //Matrix4x4 lookAtMatrix = Matrix4x4.CreateLookAt(_machCamera.Position, _machCamera.Position - _machCamera.Direction, _machCamera.CameraUp);
            //Matrix4x4 perspectiveMatrix = Matrix4x4.CreatePerspectiveFieldOfView(_machCamera.Fov, _machCamera.Viewport.X / _machCamera.Viewport.Y, _machCamera.Near, _machCamera.Far);

            //_cl.UpdateBuffer(RenderContext.ModelBuffer, 0, ref modelMatrix);
            //_cl.UpdateBuffer(RenderContext.ViewBuffer, 0, ref lookAtMatrix);
            //_cl.UpdateBuffer(RenderContext.ProjectionBuffer, 0, ref perspectiveMatrix);

            _cl.SetFramebuffer(RenderContext.GraphicsDevice.MainSwapchain.Framebuffer);

            _cl.ClearColorTarget(0, RgbaFloat.Black);
            _cl.ClearDepthStencil(1f);

            //_meshRenderer.Render(_cl);

            _imGuiRenderer.Render(RenderContext.GraphicsDevice, _cl);

            _cl.End();
            RenderContext.GraphicsDevice.SubmitCommands(_cl);
            RenderContext.GraphicsDevice.SwapBuffers(RenderContext.GraphicsDevice.MainSwapchain);
            RenderContext.GraphicsDevice.WaitForIdle();
        }

        public void DisposeResources()
        {
            _cl.Dispose();
            RenderContext.GraphicsDevice.Dispose();
        }

        public void ConfigureVeldrid(MachOptions mo)
        {
            _machOptions = mo;
        }

        public void Close()
        {
            _machWindow.Close();
        }

        public void PumpEvents()
        {
            _machWindow.PumpEvents();
        }

        public MeshRenderer LoadTestMesh()
        {
            var mesh = new Mesh(Cube.GetCubeVertices(), Cube.GetCubeIndices());
            var material = BasicMaterial.GetInstance(RenderContext);

            return new MeshRenderer(RenderContext, mesh, material);
        }
    }
}
