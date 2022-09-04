using Assimp;
using KanMach.Core.FileManager;
using KanMach.Veldrid.Model;
using KanMach.Veldrid.Rendering.Structures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Mesh = KanMach.Veldrid.Model.Mesh;

namespace KanMach.Veldrid.AssetProcessors
{
    public class AssimpMeshProcessor : AssetProcessor<List<Mesh>>
    {
        public override List<Mesh> Process(Stream stream, AssetLoaderContext context)
        {   
            var assimpContext = new AssimpContext();
            var path = context.Path;
            var scene = assimpContext.ImportFileFromStream(stream, PostProcessSteps.FlipWindingOrder, Path.GetExtension(path));

            var meshes = new List<Mesh>();

            scene.Meshes.ToList().ForEach(mesh =>
            {
                var vertices = mesh.Vertices.Select(vector => new Vector3(vector.X, vector.Y, vector.Z)).ToArray();
                var normals = mesh.Normals.Select(normal => new Vector3(normal.X, normal.Y, normal.Z)).ToArray();

                var realVertices = new VertexData[vertices.Length];
                for(var i = 0; i < realVertices.Length; i++)
                {
                    realVertices[i] = new VertexData(vertices[i], normals[i], new Vector2());
                }

                var indices = mesh.Faces.SelectMany(face => face.Indices.Select(index => Convert.ToUInt32(index)));

                meshes.Add(new Mesh(realVertices, indices.ToArray()));
            });

            

            return meshes;
        }
    }
}
