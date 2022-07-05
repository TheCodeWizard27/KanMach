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

namespace KanMach.Sample
{

    public class GraphicsSampleController : KanGameController
    {
        private readonly IVeldridService _veldridService;
        private EcsWorld _ecsWorld;
        private SceneRenderer _renderer;
        private FirstPersonCamera _camera;
        private Sdl2InputManager _inputManager;

        private double CAMERA_MIN_Y_ROT = -89 * Math.PI / 180;
        private double CAMERA_MAX_Y_ROT = 89 * Math.PI / 180;

        private List<Gamepad> _gamepads;

        public GraphicsSampleController(IVeldridService veldridService, Sdl2InputManager inputManager)
        {
            _veldridService = veldridService;
            _inputManager = inputManager;
        }

        public override void Init()
        {
            _gamepads = _inputManager.Gamepads.ToList();
            _inputManager.Gamepads.OnConnect += (gamepad) =>
            {
                _gamepads.Add(gamepad);
                Console.WriteLine("Gamepad Connected");
            };
            _inputManager.Gamepads.OnDisconnect += (gamepad) =>
            {
                _gamepads.Remove(gamepad);
            };

            _ecsWorld = new EcsWorld();

            var testObj = _ecsWorld.NewEntity();
            var transform = testObj.Get<Transform>();
            ref var renderMesh = ref testObj.Get<RenderMesh>();

            // TODO implement ressource factory of some kind.
            renderMesh.Renderer = _veldridService.LoadTestMesh();

            _renderer = new SceneRenderer(_veldridService.RenderContext);
            _renderer.Camera = _camera = new FirstPersonCamera(_renderer.ViewPort);

        }

        public override void Update(TimeSpan delta)
        {
            _veldridService.PumpEvents();
            
            var deltaS = delta.Ticks/1000000f;

            _gamepads.ForEach(gamepad =>
            {
                var rStickMovement = GetDeadZoneValue(gamepad.RightStick);

                var xRot = _camera.Rotation.X + -rStickMovement.X * deltaS;
                var yRot = (float) Math.Min(
                    Math.Max(_camera.Rotation.Y + rStickMovement.Y * deltaS, CAMERA_MIN_Y_ROT), CAMERA_MAX_Y_ROT);
                _camera.Rotation = new Vector2(xRot, yRot);
                Console.WriteLine(rStickMovement);

                var lStickMovement = GetDeadZoneValue(gamepad.LeftStick);

                var dirMatrix = Matrix4x4.CreateFromYawPitchRoll(_camera.Rotation.X, _camera.Rotation.Y, 0);
                var dir = new Vector3(-lStickMovement.X * deltaS, 0, -lStickMovement.Y * deltaS);

                //Console.WriteLine($"{xMovement} {yMovement}");
                _renderer.Camera.Position += Vector3.Transform(dir, dirMatrix);
            });

            var meshes = new List<MeshRenderer>();

            var view = _ecsWorld.View<RenderMeshView>();
            //foreach (ViewEntity<Transform, RenderMesh> entity in view)
            //{
            //    Console.WriteLine($"Entity {entity.Entity} is at {entity.Component1.Pos}");
            //}

            _renderer.Draw(view.Select(entity => entity.Component2.Renderer));

        }

        public override void Dispose()
        {
            _veldridService.DisposeResources();
        }

        private Vector2 GetDeadZoneValue(Vector2 pos)
        {
            var xMovement = pos.X > 0.2 || pos.X < -0.2 ? pos.X : 0;
            var yMovement = pos.Y > 0.2 || pos.Y < -0.2 ? pos.Y : 0;

            return new Vector2(xMovement, yMovement);
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
