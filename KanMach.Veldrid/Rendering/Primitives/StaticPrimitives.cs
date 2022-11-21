using KanMach.Veldrid.Rendering.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace KanMach.Veldrid.Rendering.Primitives
{
    public static class StaticPrimitives
    {


        public static Mesh GetPlaneMesh() 
        {
            return new Mesh(
                new VertexData[] {
                    new VertexData(new Vector3(0,0,0), new Vector3(0,0,1), new Vector2(0,0)),
                    new VertexData(new Vector3(1,0,0), new Vector3(0,0,1), new Vector2(0,0)),
                    new VertexData(new Vector3(0,0,0), new Vector3(0,0,1), new Vector2(0,0)),
                    new VertexData(new Vector3(0,0,0), new Vector3(0,0,1), new Vector2(0,0))
                }, new [] { 1u, 2u });
        
        }

    }
}
