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
        public Vector3[] Vertices { get; set; }
        public ushort[] Indices { get; set; }

        public Mesh(Vector3[] vertices, ushort[] indices)
        {
            Vertices = vertices;
            Indices = indices;
        }
    }
}
