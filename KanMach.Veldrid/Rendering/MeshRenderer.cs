using KanMach.Veldrid.Model;
using KanMach.Veldrid.Rendering.Structures;
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
        private DeviceBuffer _vertexBuffer;
        private DeviceBuffer _indexBuffer;

        public Mesh Mesh { get; private set; }
        public Material Material { get; private set; }

        RenderContext RenderContext { get; set; }

        public MeshRenderer(RenderContext context, Mesh mesh, Material material)
        {
            RenderContext = context;
            Mesh = mesh;
            Material = material;

            var factory = context.ResourceFactory;

            _vertexBuffer = factory.CreateBuffer(new BufferDescription((uint)mesh.Indices.Length * VertexData.SizeInBytes, BufferUsage.VertexBuffer));
            _indexBuffer = factory.CreateBuffer(new BufferDescription((uint)mesh.Indices.Length * sizeof(uint), BufferUsage.IndexBuffer));

            context.GraphicsDevice.UpdateBuffer(_vertexBuffer, 0, mesh.Vertices);
            context.GraphicsDevice.UpdateBuffer(_indexBuffer, 0, mesh.Indices);
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
