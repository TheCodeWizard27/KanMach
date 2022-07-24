using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Veldrid;

namespace KanMach.Veldrid.Model.Src
{
    public class Cube
    {
        public Vector3[] TransformedCube;
        public static Vector3[] GetCubeVertices()
        {
            Vector3[] vertices =
           {
                new Vector3(-0.5f, +0.5f, -0.5f),
                new Vector3(+0.5f, +0.5f, -0.5f), 
                new Vector3(+0.5f, +0.5f, +0.5f), 
                new Vector3(-0.5f, +0.5f, +0.5f), 
                // Bottom                          
                new Vector3(-0.5f,-0.5f, +0.5f),  
                new Vector3(+0.5f,-0.5f, +0.5f),  
                new Vector3(+0.5f,-0.5f, -0.5f),  
                new Vector3(-0.5f,-0.5f, -0.5f),  
                // Left                            
                new Vector3(-0.5f, +0.5f, -0.5f), 
                new Vector3(-0.5f, +0.5f, +0.5f), 
                new Vector3(-0.5f, -0.5f, +0.5f), 
                new Vector3(-0.5f, -0.5f, -0.5f), 
                // Right                           
                new Vector3(+0.5f, +0.5f, +0.5f), 
                new Vector3(+0.5f, +0.5f, -0.5f), 
                new Vector3(+0.5f, -0.5f, -0.5f), 
                new Vector3(+0.5f, -0.5f, +0.5f), 
                // Back                            
                new Vector3(+0.5f, +0.5f, -0.5f), 
                new Vector3(-0.5f, +0.5f, -0.5f), 
                new Vector3(-0.5f, -0.5f, -0.5f), 
                new Vector3(+0.5f, -0.5f, -0.5f), 
                // Front                           
                new Vector3(-0.5f, +0.5f, +0.5f), 
                new Vector3(+0.5f, +0.5f, +0.5f), 
                new Vector3(+0.5f, -0.5f, +0.5f), 
                new Vector3(-0.5f, -0.5f, +0.5f), 
            };
            return vertices;
        }
        public static uint[] GetCubeIndices()
        {
            uint[] indices = {
                0,1,2, 0,2,3,
                4,5,6, 4,6,7,
                8,9,10, 8,10,11,
                12,13,14, 12,14,15,
                16,17,18, 16,18,19,
                20,21,22, 20,22,23,
            };

            return indices;
        }
    }
}
