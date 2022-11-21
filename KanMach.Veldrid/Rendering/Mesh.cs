using KanMach.Veldrid.Rendering.Structures;
using Veldrid;

namespace KanMach.Veldrid.Rendering
{
    public class Mesh
    {

        public PrimitiveTopology Topology { get; set; } = PrimitiveTopology.TriangleList;

        public VertexData[] Vertices { get; set; }
        public uint[] Indices { get; set; }

        public Mesh(VertexData[] vertices, uint[] indices, PrimitiveTopology topology = PrimitiveTopology.TriangleList)
        {
            Vertices = vertices;
            Indices = indices;
            Topology = topology;
        }
    }
}
