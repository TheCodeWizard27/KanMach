using KanMach.Veldrid.Rendering.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace KanMach.Veldrid.Model
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
