using KanMach.Veldrid.Model;
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

            _vertexBuffer = factory.CreateBuffer(new BufferDescription((uint)(mesh.Vertices.Length * 12), BufferUsage.VertexBuffer));
            _indexBuffer = factory.CreateBuffer(new BufferDescription((uint)mesh.Indices.Length * sizeof(ushort), BufferUsage.IndexBuffer));

            context.GraphicsDevice.UpdateBuffer(_vertexBuffer, 0, mesh.Vertices);
            context.GraphicsDevice.UpdateBuffer(_indexBuffer, 0, mesh.Indices);
        }

        public void Render(CommandList cmdList) {

            Material.Prepare(cmdList);

            var modelMatrix =
                Matrix4x4.CreateTranslation(0f, 0, -0.01f)
                * Matrix4x4.CreateRotationX(0f)
                * Matrix4x4.CreateRotationY(0f)
                * Matrix4x4.CreateRotationZ(0.0001f)
                * Matrix4x4.CreateScale(1.0f);

            cmdList.UpdateBuffer(RenderContext.ModelBuffer, 0, ref modelMatrix);

            cmdList.SetVertexBuffer(0, _vertexBuffer);
            cmdList.SetIndexBuffer(_indexBuffer, IndexFormat.UInt16);

            cmdList.DrawIndexed(
                indexCount: Convert.ToUInt16(Mesh.Indices.Length),
                instanceCount: 1,
                indexStart: 0,
                vertexOffset: 0,
                instanceStart: 0);

        }

    }
}
