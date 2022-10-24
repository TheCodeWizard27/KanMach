using KanMach.Core.FileManager;
using KanMach.Veldrid.Rendering.Structures;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Numerics;
using Veldrid.ImageSharp;
using Mesh = KanMach.Veldrid.Rendering.Mesh;

namespace KanMach.Veldrid.AssetProcessors.AssimpProcessor
{
    public class AssimpModelProcessor : AssetProcessor<List<Model>>
    {

        private readonly AssimpSceneLoader _sceneLoader;

        public AssimpModelProcessor(AssimpSceneLoader sceneLoader)
        {
            _sceneLoader = sceneLoader;
        }

        public override List<Model> Process(Stream stream, AssetLoaderContext context)
        {
            var basePath = Path.GetDirectoryName(context.Path);
            var scene = _sceneLoader.Process(stream, context);

            var meshes = new List<Model>();

            scene.Meshes.ToList().ForEach(mesh =>
            {
                var vertices = mesh.Vertices.Select(vector => new Vector3(vector.X, vector.Y, vector.Z)).ToArray();
                var normals = mesh.Normals.Select(normal => new Vector3(normal.X, normal.Y, normal.Z)).ToArray();

                var realVertices = new VertexData[vertices.Length];

                ImageSharpTexture diffuseImage = null;

                var materialInfo = scene.Materials[mesh.MaterialIndex];
                if (scene.Materials[mesh.MaterialIndex].HasTextureDiffuse)
                {
                    var diffuseInfo = materialInfo.TextureDiffuse;
                    var diffusePath = diffuseInfo.FilePath;

                    if(diffusePath.StartsWith("*"))
                    {
                        var index = Convert.ToInt32(diffusePath.TrimStart('*'));

                        var imageData = scene.Textures[index].CompressedData;

                        //var imageFormat = Image.DetectFormat(imageData);
                        var test = Image.Load<Rgba32>(imageData);
                        
                        diffuseImage = new ImageSharpTexture(test);
                        //test.SaveAsPng(File.OpenWrite("test.png"));
                    }
                    else
                    {
                        diffuseImage = context
                            .Load<ImageSharpTextureProcessor, ImageSharpTexture>(Path.Combine(basePath, diffusePath));
                    }
                    
                    var uvCords = mesh.TextureCoordinateChannels[diffuseInfo.UVIndex].Select(uv => new Vector3(uv.X, uv.Y, uv.Z)).ToArray();
                    for (var i = 0; i < realVertices.Length; i++)
                    {
                        realVertices[i] = new VertexData(vertices[i], normals[i], new Vector2(uvCords[i].X, uvCords[i].Y));
                    }
                }
                else
                {
                    for (var i = 0; i < realVertices.Length; i++)
                    {
                        realVertices[i] = new VertexData(vertices[i], normals[i], Vector2.Zero);
                    }
                }
                
                var indices = mesh.Faces.SelectMany(face => face.Indices.Select(index => Convert.ToUInt32(index)));

                meshes.Add(new Model()
                {
                    Mesh = new Mesh(realVertices, indices.ToArray()),
                    MaterialData = new MaterialData()
                    {
                        DiffuseImage = diffuseImage
                    }
                });
            });

            return meshes;
        }
    }
}
