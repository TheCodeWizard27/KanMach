using Assimp;
using KanMach.Core.FileManager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanMach.Veldrid.AssetProcessors.AssimpProcessor
{
    public class AssimpSceneLoader : AssetProcessor<Scene>
    {
        public override Scene Process(Stream stream, AssetLoaderContext context)
        {
            using var assimpContext = new AssimpContext();
            var scene = assimpContext.ImportFileFromStream(stream, PostProcessSteps.FlipWindingOrder, Path.GetExtension(context.Path));

            return scene;
        }
    }
}
