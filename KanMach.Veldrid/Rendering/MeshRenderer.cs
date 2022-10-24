using KanMach.Veldrid.EmbeddedShaders;
using KanMach.Veldrid.Rendering;
using KanMach.Veldrid.Rendering.Structures;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Veldrid;

namespace KanMach.Veldrid.Components
{
    public class MeshRenderer
    {
        private readonly DisposeCollector _disposeCollector = new DisposeCollector();

        private DeviceBuffer _vertexBuffer;
        private DeviceBuffer _indexBuffer;

        public Mesh Mesh { get; private set; }
        public Material Material { get; private set; }

        RenderContext RenderContext { get; set; }

        public MeshRenderer(RenderContext context, Model model) : this(context, model.Mesh, BasicMaterial.NewInstance(context))
        {
            if(model.MaterialData.DiffuseImage != null)
            {
                var texture = model.MaterialData.DiffuseImage.CreateDeviceTexture(context.GraphicsDevice, context.ResourceFactory);
                ((BasicMaterial)Material).DiffuseTexture = context.ResourceFactory.CreateTextureView(texture);
            }
            
        }

        public MeshRenderer(RenderContext context, Mesh mesh, Material material)
        {
            RenderContext = context;
            Mesh = mesh;
            Material = material;

            var factory = context.ResourceFactory;

            _vertexBuffer = factory.CreateBuffer(new BufferDescription((uint)Mesh.Indices.Length * VertexData.SizeInBytes, BufferUsage.VertexBuffer));
            _indexBuffer = factory.CreateBuffer(new BufferDescription((uint)Mesh.Indices.Length * sizeof(uint), BufferUsage.IndexBuffer));

            context.GraphicsDevice.UpdateBuffer(_vertexBuffer, 0, Mesh.Vertices);
            context.GraphicsDevice.UpdateBuffer(_indexBuffer, 0, Mesh.Indices);
        }

        public void Render(CommandList cmdList) {

            Material.Prepare(cmdList);

            cmdList.SetVertexBuffer(0, _vertexBuffer);
            cmdList.SetIndexBuffer(_indexBuffer, IndexFormat.UInt32);

            cmdList.DrawIndexed(
                indexCount: Convert.ToUInt32(Mesh.Indices.Length),
                instanceCount: 1,
                indexStart: 0,
                vertexOffset: 0,
                instanceStart: 0);

        }

    }
}
