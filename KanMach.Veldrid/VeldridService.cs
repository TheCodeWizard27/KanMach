using ImGuiNET;
using KanMach.Veldrid.Components;
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
        private MachCamera _machCamera;
        private MachOptions _machOptions;
        private GraphicsDevice _graphicsDevice;
        private ImGuiRenderer _imGuiRenderer;
        private CommandList _cl;
        private DeviceBuffer _modelBuffer;
        private DeviceBuffer _indexBuffer;
        private DeviceBuffer _vertexBuffer;
        private DeviceBuffer _viewBuffer;
        private DeviceBuffer _projectionBuffer;
        private MachShader _shader;
        private MachPipeline _pipeline;
        private Vector3[] _vertices;
        private ushort[] _indices;
        private float tick;

        public event OnCloseHandler OnClose;

        public VeldridService(MachOptions machOptions)
        {
            _machOptions = machOptions ?? new MachOptions();
        }

        public void Init()
        {
            _machWindow = new MachWindow(_machOptions);
            _machWindow.Closed += () => OnClose?.Invoke();

            _graphicsDevice = VeldridStartup.CreateGraphicsDevice(_machWindow,  _machOptions.GraphicsDeviceOptions);
            _imGuiRenderer = new ImGuiRenderer(
                _graphicsDevice,
                _graphicsDevice.MainSwapchain.Framebuffer.OutputDescription,
                _machWindow.Width,
                _machWindow.Height);

            _machWindow.Resized += _machWindow_Resized;

            _machCamera = new MachCamera(_machWindow);
            _indices = Cube.GetCubeIndices();
            _vertices = Cube.GetCubeVertices();

            CreateResources();
        }

        private void _machWindow_Resized()
        {
            _imGuiRenderer.WindowResized(_machWindow.Width, _machWindow.Height);
            _graphicsDevice.MainSwapchain.Resize((uint)_machWindow.Width, (uint)_machWindow.Height);
        }

        public void CreateResources()
        {
            ResourceFactory factory = _graphicsDevice.ResourceFactory;

            _modelBuffer = factory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer));
            _viewBuffer = factory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer));
            _projectionBuffer = factory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer));

            _vertexBuffer = factory.CreateBuffer(new BufferDescription((uint)(_vertices.Length * 12), BufferUsage.VertexBuffer));
            _indexBuffer = factory.CreateBuffer(new BufferDescription((uint)_indices.Length * sizeof(ushort), BufferUsage.IndexBuffer));

            _graphicsDevice.UpdateBuffer(_vertexBuffer, 0, _vertices);
            _graphicsDevice.UpdateBuffer(_indexBuffer, 0, _indices);

            _shader = new MachShader(factory, _modelBuffer, _viewBuffer, _projectionBuffer);
            _pipeline = new MachPipeline(_graphicsDevice, _shader );

            _cl = factory.CreateCommandList();

        }

        public void Draw()
        {
            var snapshot = _machWindow.PumpEvents();
            _imGuiRenderer.Update(1f / 60f, snapshot);

            if (ImGui.Begin("Test Window"))
            {
                ImGui.TreeNode("test");

                var test = new Vector4();
                ImGui.ColorEdit4("TestColor", ref test);
                ImGui.Text("Hello");
                if (ImGui.Button("Quit"))
                {
                    _machWindow.Close();
                }
            }

            ImGui.End();

            _cl.Begin();
            tick += 0.0001f;
            Matrix4x4 modelMatrix =
                Matrix4x4.CreateTranslation(0f, 0, -0.01f)
                * Matrix4x4.CreateRotationX(0f)
                * Matrix4x4.CreateRotationY(0f)
                * Matrix4x4.CreateRotationZ(tick)
                * Matrix4x4.CreateScale(1.0f);

            Matrix4x4 lookAtMatrix = Matrix4x4.CreateLookAt(_machCamera.Position, _machCamera.Position - _machCamera.Direction, _machCamera.CameraUp);
            Matrix4x4 perspectiveMatrix = Matrix4x4.CreatePerspectiveFieldOfView(_machCamera.Fov, _machCamera.Width / _machCamera.Height, _machCamera.Near, _machCamera.Far);

            _cl.UpdateBuffer(_modelBuffer, 0, ref modelMatrix);
            _cl.UpdateBuffer(_viewBuffer, 0, ref lookAtMatrix);
            _cl.UpdateBuffer(_projectionBuffer, 0, ref perspectiveMatrix);

            _cl.SetFramebuffer(_graphicsDevice.MainSwapchain.Framebuffer);

            _cl.ClearColorTarget(0, RgbaFloat.Black);
            _cl.ClearDepthStencil(1f);

            _cl.SetPipeline(_pipeline.Pipeline);
            _cl.SetGraphicsResourceSet(0, _shader.ModelSet);
            _cl.SetGraphicsResourceSet(1, _shader.VertexSet);
            _cl.SetVertexBuffer(0, _vertexBuffer);
            _cl.SetIndexBuffer(_indexBuffer, IndexFormat.UInt16);

            _cl.DrawIndexed(
                indexCount: 36,
                instanceCount: 1,
                indexStart: 0,
                vertexOffset: 0,
                instanceStart: 0);

            _imGuiRenderer.Render(_graphicsDevice, _cl);
            _cl.End();
            _graphicsDevice.SubmitCommands(_cl);
            _graphicsDevice.SwapBuffers(_graphicsDevice.MainSwapchain);
            _graphicsDevice.WaitForIdle();
        }

        public void DisposeResources()
        {
            _pipeline.Pipeline.Dispose();
            _cl.Dispose();
            _vertexBuffer.Dispose();
            _indexBuffer.Dispose();
            _graphicsDevice.Dispose();
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
    }
}
