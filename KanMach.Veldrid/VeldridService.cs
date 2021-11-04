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

namespace KanMach.Veldrid
{
    public class VeldridService : IVeldridService
    {
        private MachWindow MachWindow;
        private MachCamera MachCamera;
        private MachOptions MachOptions;
        private GraphicsDevice _graphicsDevice;
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
        


        public VeldridService()
        {

        }

        public void InitService()
        {
            MachWindow = new MachWindow(MachOptions);


            _graphicsDevice = VeldridStartup.CreateGraphicsDevice(MachWindow, MachOptions.GDOpt);
            MachCamera = new MachCamera(MachWindow);
            _indices = Cube.GetCubeIndices();
            _vertices = Cube.GetCubeVertices();

            CreateResources();

            while (MachWindow.Exists)
            {
                MachWindow.PumpEvents();
                Draw();
            }
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
            _cl.Begin();
            tick += 0.0001f;
            Matrix4x4 modelMatrix =
                Matrix4x4.CreateTranslation(0f, 0, -0.01f)
                * Matrix4x4.CreateRotationX(0f)
                * Matrix4x4.CreateRotationY(0f)
                * Matrix4x4.CreateRotationZ(tick)
                * Matrix4x4.CreateScale(1.0f);

            Matrix4x4 lookAtMatrix = Matrix4x4.CreateLookAt(MachCamera.Position, MachCamera.Position - MachCamera.Direction, MachCamera.CameraUp);
            Matrix4x4 perspectiveMatrix = Matrix4x4.CreatePerspectiveFieldOfView(MachCamera.Fov, MachCamera.Width / MachCamera.Height, MachCamera.Near, MachCamera.Far);

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
            MachOptions = mo;
        }
    }
}
