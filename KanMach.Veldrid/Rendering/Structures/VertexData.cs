using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace KanMach.Veldrid.Rendering.Structures
{
    public struct VertexData
    {

        public const int SizeInBytes = 32;

        Vector3 Vertex;
        Vector3 Normal;
        Vector2 UV;

        public VertexData(Vector3 vertex, Vector3 normal, Vector2 uv)
        {
            Vertex = vertex;
            Normal = normal;
            UV = uv;
        }


    }
}
