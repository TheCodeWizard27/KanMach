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
using KanMach.Veldrid.Graphics;
using System.IO;
using KanMach.Core.FileManager;
using KanMach.Veldrid.Model;
using KanMach.Veldrid.EmbeddedShaders;

namespace KanMach.Sample
{

    public class GraphicsSampleController : KanGameController
    {

        private readonly IVeldridService _veldridService;
        private readonly AssetLoader<FileSourceHandler> _assetLoader;

        private EcsWorld _ecsWorld;
        private SceneRenderer _renderer;
        private FirstPersonCamera _camera;
        private Sdl2InputManager _inputManager;
        private RenderMeshView _renderMeshView;

        private double CAMERA_MIN_Y_ROT = -89 * Math.PI / 180;
        private double CAMERA_MAX_Y_ROT = 89 * Math.PI / 180;

        public GraphicsSampleController(
            IVeldridService veldridService,
            AssetLoader<FileSourceHandler> assetLoader,
            Sdl2InputManager inputManager)
        {
            _veldridService = veldridService;
            _assetLoader = assetLoader;
            _inputManager = inputManager;
        }

        public override void Init()
        {
            // maps are not saved on git.
            var importedMeshes = _assetLoader.Load<List<Mesh>>("SampleModels/cube.dae");

            _ecsWorld = new EcsWorld();

            importedMeshes.ForEach((mesh) =>
            {
                var entity = _ecsWorld.NewEntity();
                ref var transform = ref entity.Get<Transform>();
                transform.Scale = new Vector3(1.0f, 1.0f, 1.0f);
                transform.Rotation.X = (float)(180 * Math.PI / 180);

                ref var renderMesh = ref entity.Get<RenderMesh>();

                renderMesh.Renderer = new MeshRenderer(
                    _veldridService.RenderContext,
                    mesh,
                    PhongMaterial.GetInstance(_veldridService.RenderContext));

                var light = _ecsWorld.NewEntity();
                ref var lightTransform = ref light.Get<Transform>();
                lightTransform.Position = new Vector3(2, 0, 30);
                lightTransform.Scale = new Vector3(0.1f);
                lightTransform.Rotation.X = (float)(180 * Math.PI / 180);

                ref var lightRenderMesh = ref light.Get<RenderMesh>();
                var lightMaterial = BasicMaterial.GetInstance(_veldridService.RenderContext);
                lightMaterial.Color = new Vector3(0, 1, 2);

                lightRenderMesh.Renderer = new MeshRenderer(
                    _veldridService.RenderContext,
                    mesh,
                    lightMaterial
                    );
            });
                        
            _renderer = new SceneRenderer(_veldridService.RenderContext);
            _renderer.Camera = _camera = new FirstPersonCamera(_renderer.ViewPort);
            _camera.Position = new Vector3(0, 0, 2);

            _renderMeshView = _ecsWorld.View<RenderMeshView>();

        }

        public override void Update(TimeSpan delta)
        {
            _veldridService.PumpEvents();
            
            // Maybe clean this up and also drossle the
            // engine so that it doesn't run at unlimited speed all the time.
            var deltaValue = delta.Ticks/1000000f;
            UpdateFpsCamera(deltaValue);

            _renderer.Draw(_renderMeshView.Select(entity => (entity.Component2.Renderer, entity.Component1)));

        }

        public override void Dispose()
        {
            _veldridService.DisposeResources();
        }

        private void UpdateFpsCamera(float deltaValue)
        {
            _inputManager.Gamepads.ToList().ForEach(gamepad =>
            {
                var rStickMovement = gamepad.RightStick;

                var xRot = _camera.Rotation.X + rStickMovement.X * deltaValue;
                var yRot = (float)Math.Min(
                    Math.Max(_camera.Rotation.Y + rStickMovement.Y * deltaValue, CAMERA_MIN_Y_ROT), CAMERA_MAX_Y_ROT);
                _camera.Rotation = new Vector2(xRot, yRot);

                var lStickMovement = gamepad.LeftStick;

                var dirMatrix = Matrix4x4.CreateFromYawPitchRoll(_camera.Rotation.X, _camera.Rotation.Y, 0);
                var dir = new Vector3(-lStickMovement.X * deltaValue, 0, lStickMovement.Y * deltaValue);

                _renderer.Camera.Position += Vector3.Transform(dir, dirMatrix);
            });
        }

        internal struct RenderMesh
        {
            public MeshRenderer Renderer;
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
