using KanMach.Veldrid.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Veldrid;
using Veldrid.Sdl2;

namespace KanMach.Veldrid.Graphics.Cameras
{
    public abstract class Camera
    {
        private GraphicsDevice _graphicsDevice;

        public float Fov { get; set; }
        public float Near { get; set; }
        public float Far { get; set; }

        public abstract Vector3 Position { get; set; }
        public abstract Vector3 Target { get; set; }
        public Vector3 CameraUp { get; set; } = Vector3.UnitY;

        public Vector2 Viewport { get; set; }

        public Camera(RenderContext context, Vector2 viewport)
        {
            _graphicsDevice = context.GraphicsDevice;

            Position = new Vector3(0f);

            Near = 1f;
            Far = 1000f;
            Fov = 1f;

            Viewport = viewport;
        }

        public Matrix4x4 GetView() {
            return Matrix4x4.CreateLookAt(Position, Target, CameraUp); 
        }

        public Matrix4x4 GetPerspective()
        {
            return Matrix4x4.CreatePerspectiveFieldOfView(Fov, Viewport.X / Viewport.Y, Near, Far);
        }

    }

    public class SimpleCamera : Camera
    {
        public override Vector3 Position { get; set; }
        public override Vector3 Target { get; set; }

        public SimpleCamera(RenderContext context, Vector2 viewport) : base(context, viewport)
        {
        }
        
    }
}
