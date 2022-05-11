using KanMach.Core;
using KanMach.Core.Ecs;
using KanMach.Core.Ecs.Extensions;
using KanMach.Core.Ecs.View;
using KanMach.Veldrid;
using KanMach.Veldrid.Components;
using KanMach.Veldrid.Input;
using KanMach.Veldrid.Rendering;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Numerics;

namespace KanMach.Sample
{

    public class GraphicsSampleController : KanGameController
    {
        private readonly IVeldridService _veldridService;
        private EcsWorld _ecsWorld;
        private SceneRenderer _renderer;

        public GraphicsSampleController(IVeldridService veldridService, Sdl2InputManager inputManager)
        {
            _veldridService = veldridService;
        }

        public override void Init()
        {
            _ecsWorld = new EcsWorld();

            var testObj = _ecsWorld.NewEntity();
            var transform = testObj.Get<Transform>();
            ref var renderMesh = ref testObj.Get<RenderMesh>();
            // TODO implement ressource factory of some kind.
            renderMesh.Renderer = _veldridService.LoadTestMesh();

            _renderer = new SceneRenderer(_veldridService.RenderContext);

        }

        public override void Update(TimeSpan delta)
        {

            _veldridService.PumpEvents();

            //var cmd = _veldridService.BeginDraw();

            //var view = _ecsWorld.View<RenderMeshView>();
            //foreach (var entity in view)
            //{
            //    view.GetRenderMesh(entity).Renderer.Render(cmd);
            //}

            //_veldridService.EndDraw();

            var meshes = new List<MeshRenderer>();

            var view = _ecsWorld.View<RenderMeshView>();
            view.First(entity => entity.Component1.Pos.X >= 3);
            foreach (ViewEntity<Transform, RenderMesh> entity in view)
            {
                Console.WriteLine($"Entity {entity.Entity} is at {entity.Component1.Pos}");
            }

            _renderer.Draw(view.Select(entity => entity.Component2.Renderer));

        }

        public override void Dispose()
        {
            _veldridService.DisposeResources();
        }

        internal struct RenderMesh
        {
            public MeshRenderer Renderer;
        }

        internal struct Transform
        {
            public Vector3 Pos;
        }

        internal class RenderMeshView : EcsView<Transform, RenderMesh>
        {

            public ref Transform GetTransform(in int id) => ref Get1(id);
            public ref RenderMesh GetRenderMesh(in int id) => ref Get2(id);

            public RenderMeshView(EcsWorld world) : base(world)
            {
            }

        }
    }
}
