using KanMach.Veldrid.Components;
using KanMach.Veldrid.Graphics;
using KanMach.Veldrid.Model.Src;
using System.Numerics;
using Veldrid;
using Veldrid.StartupUtilities;

namespace KanMach.Veldrid
{
    public class VeldridService
    {
        private MachWindow MachWindow;
        private MachCamera MachCamera;
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


        public VeldridService()
        {
            MachWindow = new MachWindow(100, 100, 960, 560, "KanMach");

            GraphicsDeviceOptions options = new GraphicsDeviceOptions
            {
                PreferStandardClipSpaceYDirection = true,
                PreferDepthRangeZeroToOne = true
            };

            _graphicsDevice = VeldridStartup.CreateGraphicsDevice(MachWindow.window, options);
            _indices = Cube.GetCubeIndices();
            _vertices = Cube.GetCubeVertices();

            CreateResources();

            while (MachWindow.window.Exists)
            {
                MachWindow.window.PumpEvents();
            }


        }

        private void CreateResources()
        {
            ResourceFactory factory = _graphicsDevice.ResourceFactory;


            _modelBuffer = factory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer));
            _viewBuffer = factory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer));
            _projectionBuffer = factory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer));
            _indexBuffer = factory.CreateBuffer(new BufferDescription(64, BufferUsage.VertexBuffer));
            _vertexBuffer = factory.CreateBuffer(new BufferDescription(64, BufferUsage.IndexBuffer));

            _graphicsDevice.UpdateBuffer(_vertexBuffer, 0, _vertices);
            _graphicsDevice.UpdateBuffer(_indexBuffer, 0, _indices);

            _shader = new MachShader(factory, _modelBuffer, _viewBuffer, _projectionBuffer);
            _pipeline = new MachPipeline(_graphicsDevice, _shader );

            _cl = factory.CreateCommandList();

        }
    }
}
