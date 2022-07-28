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
using System.Threading.Tasks;

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

        private PhongMaterial _cubeMaterial;

        private float FRAME_RATE = 1000 / 60;
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

            var mesh = importedMeshes.First();

            var light = _ecsWorld.NewEntity();
            ref var lightTransform = ref light.Get<Transform>();
            lightTransform.Position = new Vector3(2, 15, 30);
            lightTransform.Scale = new Vector3(0.1f);

            ref var lightRenderMesh = ref light.Get<RenderMesh>();
            var lightMaterial = BasicMaterial.NewInstance(_veldridService.RenderContext);
            lightMaterial.Color = new Vector3(1, 1, 1);

            lightRenderMesh.Renderer = new MeshRenderer(
                _veldridService.RenderContext,
                mesh,
                lightMaterial
                );

            var entity = _ecsWorld.NewEntity();
            ref var transform = ref entity.Get<Transform>();
            transform.Scale = new Vector3(1.0f, 1.0f, 1.0f);

            ref var renderMesh = ref entity.Get<RenderMesh>();

            var cubeMaterial = PhongMaterial.NewInstance(_veldridService.RenderContext);
            //cubeMaterial.LightPos = lightTransform.Position;
            cubeMaterial.LightPos = new Vector3(2, 15, 30);

            renderMesh.Renderer = new MeshRenderer(
                _veldridService.RenderContext,
                mesh,
                cubeMaterial
                );

            _renderer = new SceneRenderer(_veldridService.RenderContext);
            _renderer.Camera = _camera = new FirstPersonCamera(_renderer.ViewPort);
            _camera.Position = new Vector3(0, 0, 2);

            _renderMeshView = _ecsWorld.View<RenderMeshView>();

        }

        public override void Update(TimeSpan delta)
        {
            // Clean up doesnt work smoothly.
            //var waitTime = (int)Math.Max(FRAME_RATE - delta.Milliseconds, 0);
            //if (waitTime > 0) Task.Delay(waitTime).Wait();
            Task.Delay(0);

            _veldridService.PumpEvents();
            
            var deltaValue = delta.Ticks / 1000000f;

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

                var xRot = _camera.Rotation.X + -rStickMovement.X * deltaValue;
                var yRot = (float)Math.Min(
                    Math.Max(_camera.Rotation.Y + -rStickMovement.Y * deltaValue, CAMERA_MIN_Y_ROT), CAMERA_MAX_Y_ROT);
                _camera.Rotation = new Vector2(xRot, yRot);

                var lStickMovement = gamepad.LeftStick;

                var dirMatrix = Matrix4x4.CreateFromYawPitchRoll(_camera.Rotation.X, _camera.Rotation.Y, 0);
                var dir = new Vector3(lStickMovement.X * deltaValue, 0, lStickMovement.Y * deltaValue);

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
