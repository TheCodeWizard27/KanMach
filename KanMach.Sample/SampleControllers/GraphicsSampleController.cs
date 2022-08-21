﻿using KanMach.Core;
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
using Veldrid;
using ImGuiNET;

namespace KanMach.Sample
{

    public class GraphicsSampleController : KanGameController
    {

        private readonly IVeldridService _veldridService;
        private readonly AssetLoader<FileSourceHandler> _assetLoader;

        private EcsWorld _ecsWorld;
        private SceneRenderer _renderer;
        private ImGuiRenderer _imGuiRenderer;
        private FirstPersonCamera _camera;
        private Sdl2InputManager _inputManager;
        private RenderMeshView _renderMeshView;

        private PhongMaterial _cubeMaterial;
        private Entity _cubeEntity;

        private BasicMaterial _lightMaterial;
        private Entity _lightEntity;

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
            _renderer = new SceneRenderer(_veldridService.RenderContext);
            _renderer.Camera = _camera = new FirstPersonCamera(_veldridService.RenderContext, _renderer.ViewPort);
            _camera.Position = new Vector3(0, 0, 0);

            SetupTestScene();
            SetupImGui();
        }

        public override void Update(TimeSpan delta)
        {
            // Clean up doesnt work smoothly.
            //var waitTime = (int)Math.Max(FRAME_RATE - delta.Milliseconds, 0);
            //if (waitTime > 0) Task.Delay(waitTime).Wait();
            Task.Delay(0);

            _imGuiRenderer.Update(delta.Seconds, _veldridService.CurrentInputSnapshot);
            
            var deltaValue = delta.Ticks / 1000000f;

            UpdateFpsCamera(deltaValue);

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

            ImGui.DragFloat3("Specular Strength", ref _cubeMaterial.MaterialProperties.Specular);

            ImGui.DragFloat("Shininess", ref _cubeMaterial.MaterialProperties.Shininess);
            ImGui.EndGroup();

            #endregion

            var commandList = _veldridService.RenderContext.BeginDraw();

            var renderables = _renderMeshView.Select(entity => (entity.Component2.Renderer, entity.Component1));
            _renderer.Draw(renderables, commandList);
            _imGuiRenderer.Render(_veldridService.RenderContext.GraphicsDevice, commandList);

            _veldridService.RenderContext.EndDraw();
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
                var yRot = (float)Math.Clamp(_camera.Rotation.Y + -rStickMovement.Y * deltaValue, CAMERA_MIN_Y_ROT, CAMERA_MAX_Y_ROT); ;
                _camera.Rotation = new Vector2(xRot, yRot);

                var lStickMovement = gamepad.LeftStick;

                var dirMatrix = Matrix4x4.CreateFromYawPitchRoll(_camera.Rotation.X, _camera.Rotation.Y, 0);
                var dir = new Vector3(lStickMovement.X * deltaValue, 0, lStickMovement.Y * deltaValue);

                _renderer.Camera.Position += Vector3.Transform(dir, dirMatrix);
            });
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
            var cubeMeshes = _assetLoader.Load<List<Mesh>>("SampleModels/cube.dae");
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
            _lightMaterial = BasicMaterial.NewInstance(_veldridService.RenderContext);
            _lightMaterial.Color = new Vector3(1, 1, 1);
            lightRenderMesh.Renderer = new MeshRenderer(
                _veldridService.RenderContext,
                cubeMesh,
                _lightMaterial
                );


            // Create Cube Model
            _cubeEntity = _ecsWorld.NewEntity();
            ref var cubeTransform = ref _cubeEntity.Get<Transform>();
            cubeTransform.Scale = new Vector3(1.0f);

            _cubeMaterial = PhongMaterial.NewInstance(_veldridService.RenderContext);
            _cubeMaterial.LightPos = lightTransform.Position;
            ref var renderMesh = ref _cubeEntity.Get<RenderMesh>();
            renderMesh.Renderer = new MeshRenderer(
                _veldridService.RenderContext,
                cubeMesh,
                _cubeMaterial
                );
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
