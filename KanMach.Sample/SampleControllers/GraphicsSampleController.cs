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
using KanMach.Core.FileManager;
using KanMach.Veldrid.EmbeddedShaders;
using Veldrid;
using ImGuiNET;
using KanMach.Core.Helper;
using KanMach.Veldrid.AssetProcessors.AssimpProcessor;
using KanMach.Veldrid.Rendering.Structures;
using KanMach.Veldrid.Rendering.Renderers;
using KanMach.Veldrid.Graphics.Cameras;

namespace KanMach.Sample
{

    public class GraphicsSampleController : KanGameController
    {

        private readonly IVeldridService _veldridService;
        private readonly AssetLoader<FileSourceHandler> _assetLoader;

        private EcsWorld _ecsWorld;
        private SceneRenderContext _sceneRenderContext;
        private ForwardSceneRenderer _renderer;
        private ImGuiRenderer _imGuiRenderer;
        private FirstPersonCamera _camera;
        private IVeldridInputManager _inputManager;
        private RenderMeshView _renderMeshView;

        private BasicMaterial _cubeMaterial;
        private Entity _cubeEntity;

        private FlatMaterial _lightMaterial;
        private Entity _lightEntity;

        private bool _mouseCameraEnabled = false;

        private int FRAME_RATE = 60;
        private double CAMERA_MIN_Y_ROT = -89 * Math.PI / 180;
        private double CAMERA_MAX_Y_ROT = 89 * Math.PI / 180;

        public GraphicsSampleController(
            IVeldridService veldridService,
            AssetLoader<FileSourceHandler> assetLoader,
            IVeldridInputManager inputManager)
        {
            _veldridService = veldridService;
            _assetLoader = assetLoader;
            _inputManager = inputManager;
        }

        public override void Init()
        {
            _inputManager.Keyboard.OnButtonReleased += Keyboard_OnButtonReleased;

            _sceneRenderContext = new SceneRenderContext(_veldridService.RenderContext);

            _renderer = new ForwardSceneRenderer(_sceneRenderContext);
            _renderer.Camera = _camera = new FirstPersonCamera(_veldridService.RenderContext, _renderer.ViewPort);
            _camera.Position = new Vector3(0, 0, 0);

            SetupTestScene();
            SetupImGui();
        }

        private void Keyboard_OnButtonReleased(Key button)
        {
            //Console.WriteLine(button);
        }

        public override void Update(FrameTime delta)
        {
            var start = DateTime.Now;

            _imGuiRenderer.Update(delta.GetDelta(FRAME_RATE), _veldridService.CurrentInputSnapshot);
            
            HandleInput(delta.GetDelta(FRAME_RATE));

            #region Begin ImGui Debug Code
            //  TODO Improve

            ImGui.BeginGroup();
            ImGui.Text("- Camera");

            var pos = _camera.Position;
            ImGui.DragFloat3("Position", ref pos);
            _camera.Position = pos;

            ImGui.EndGroup();

            ImGui.BeginGroup();
            ImGui.Text("- Light");
            var lightColor = _lightMaterial.Color;
            
            ImGui.ColorEdit3("LightColor", ref lightColor);
            _lightMaterial.Color = lightColor;
            _cubeMaterial.LightColor = lightColor;

            ImGui.DragFloat3("LightPos", ref _lightEntity.Get<Transform>().Position);
            _cubeMaterial.LightPos = _lightEntity.Get<Transform>().Position;
            
            ImGui.EndGroup();

            ImGui.BeginGroup();
            ImGui.Text("- Cube");
            
            ImGui.ColorEdit3("Diffuse", ref _cubeMaterial.MaterialProperties.Diffuse);

            var ambientColor = _cubeMaterial.AmbientColor;
            ImGui.ColorEdit3("AmbientColor", ref ambientColor);
            _cubeMaterial.AmbientColor = ambientColor;

            ImGui.DragFloat3("Specular Strength", ref _cubeMaterial.MaterialProperties.Specular, 0.01f);

            ImGui.DragFloat("Shininess", ref _cubeMaterial.MaterialProperties.Shininess);
            ImGui.EndGroup();

            #endregion

            var commandList = _veldridService.RenderContext.BeginDraw();

            var renderables = _renderMeshView.Select(entity => (entity.Component2.Renderer, entity.Component1));
            _renderer.Draw(renderables, commandList);
            _imGuiRenderer.Render(_veldridService.RenderContext.GraphicsDevice, commandList);

            _veldridService.RenderContext.EndDraw();

            FrameHelper.SleepPrecise(FRAME_RATE, DateTime.Now - start);
        }

        public override void Dispose()
        {
            _veldridService.DisposeResources();
        }

        private void HandleInput(float deltaValue)
        {
            #region Gamepad input
            _inputManager.Gamepads.ToList().ForEach(gamepad =>
            {
                var rStickMovement = gamepad.RightStick;
                var stickSensitivity = 0.05f;

                var xRot = _camera.Rotation.X + -rStickMovement.X * stickSensitivity * deltaValue;
                var yRot = (float)Math.Clamp(_camera.Rotation.Y + -rStickMovement.Y * stickSensitivity * deltaValue, CAMERA_MIN_Y_ROT, CAMERA_MAX_Y_ROT);
                _camera.Rotation = new Vector2(xRot, yRot);

                var lStickMovement = gamepad.LeftStick;

                var dirMatrix = Matrix4x4.CreateFromYawPitchRoll(_camera.Rotation.X, _camera.Rotation.Y, 0);
                var dir = new Vector3(lStickMovement.X * deltaValue, 0, lStickMovement.Y * deltaValue);

                _renderer.Camera.Position += Vector3.Transform(dir, dirMatrix);
            });
            #endregion

            #region Keyboard input
            if (_mouseCameraEnabled)
            {
                var viewport = _veldridService.Viewport;
                var mouseMovement = new Vector2(viewport.X/2, viewport.Y/2) - _inputManager.Mouse.Position;
                var mouseSensitivity = 0.025f;

                var xRot = _camera.Rotation.X + mouseMovement.X * mouseSensitivity * deltaValue;
                var yRot = Math.Clamp(_camera.Rotation.Y + mouseMovement.Y * mouseSensitivity * deltaValue, CAMERA_MIN_Y_ROT, CAMERA_MAX_Y_ROT);
                _camera.Rotation = new Vector2((float)xRot, (float)yRot);

                var wasdMovement = Vector2.Zero;

                if (_inputManager.Keyboard.IsButtonDown(Key.W))
                {
                    wasdMovement += -Vector2.UnitY;
                }
                if (_inputManager.Keyboard.IsButtonDown(Key.A))
                {
                    wasdMovement += -Vector2.UnitX;
                }
                if (_inputManager.Keyboard.IsButtonDown(Key.S))
                {
                    wasdMovement += Vector2.UnitY;
                }
                if (_inputManager.Keyboard.IsButtonDown(Key.D))
                {
                    wasdMovement += Vector2.UnitX;
                }

                var dirMatrix = Matrix4x4.CreateFromYawPitchRoll(_camera.Rotation.X, _camera.Rotation.Y, 0);
                var dir = new Vector3(wasdMovement.X * deltaValue, 0, wasdMovement.Y * deltaValue);
                _renderer.Camera.Position += Vector3.Transform(dir, dirMatrix);
            }

            if(_inputManager.Keyboard.IsButtonClicked(Key.Escape)) {
                _veldridService.KeepMouseCentered = !_veldridService.KeepMouseCentered;
                _veldridService.MouseVisible = !_veldridService.MouseVisible;
                _mouseCameraEnabled = !_mouseCameraEnabled;
            }

            if(_inputManager.Keyboard.IsButtonClicked(Key.X))
            {
                _veldridService.Close();
            }
            #endregion
        }

        private void SetupImGui()
        {
            var graphicsDevice = _veldridService.RenderContext.GraphicsDevice;
            _imGuiRenderer = new ImGuiRenderer(
                graphicsDevice,
                graphicsDevice.MainSwapchain.Framebuffer.OutputDescription,
                (int)graphicsDevice.MainSwapchain.Framebuffer.Width,
                (int)graphicsDevice.MainSwapchain.Framebuffer.Height);
        }

        private void SetupTestScene()
        {
            // Models are not saved on git.
            var cubeMeshes = _assetLoader.Load<AssimpModelProcessor, List<Model>>("SampleModels/teapot.fbx");
            //var importedMeshes = _assetLoader.Load<List<Mesh>>("SampleModels/darksouls.dae");

            _ecsWorld = new EcsWorld();
            _renderMeshView = _ecsWorld.View<RenderMeshView>();

            var cubeMesh = cubeMeshes.First();
            //var mesh = importedMeshes.Skip(1).First();

            // Create Light Entity
            _lightEntity = _ecsWorld.NewEntity();
            ref var lightTransform = ref _lightEntity.Get<Transform>();
            lightTransform.Position = new Vector3(0, 3, 0);
            lightTransform.Scale = new Vector3(0.1f);

            ref var lightRenderMesh = ref _lightEntity.Get<RenderMesh>();
            _lightMaterial = FlatMaterial.NewInstance(_veldridService.RenderContext);
            _lightMaterial.Color = new Vector3(1, 1, 1);
            lightRenderMesh.Renderer = new MeshRenderer(
                _veldridService.RenderContext,
                cubeMesh.Mesh,
                _lightMaterial
                );

            // Create Cube Model
            _cubeEntity = _ecsWorld.NewEntity();
            ref var cubeTransform = ref _cubeEntity.Get<Transform>();
            cubeTransform.Scale = new Vector3(1.0f);

            ref var renderMesh = ref _cubeEntity.Get<RenderMesh>();
            renderMesh.Renderer = new MeshRenderer(
                _veldridService.RenderContext,
                cubeMesh
                );
            _cubeMaterial = (BasicMaterial) renderMesh.Renderer.Material;
            _cubeMaterial.LightPos = lightTransform.Position;
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
