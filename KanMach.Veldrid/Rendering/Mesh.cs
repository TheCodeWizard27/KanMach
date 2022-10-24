using KanMach.Veldrid.Rendering.Structures;

namespace KanMach.Veldrid.Rendering
{
    public class Mesh
    {
        public VertexData[] Vertices { get; set; }
        public uint[] Indices { get; set; }

        public Mesh(VertexData[] vertices, uint[] indices)
        {
            Vertices = vertices;
            Indices = indices;
        }
    }
}
