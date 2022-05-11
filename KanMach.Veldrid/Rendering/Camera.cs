using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

using Veldrid.Sdl2;

namespace KanMach.Veldrid.Graphics
{
    public class Camera
    {
        public float Fov { get; set; }
        public float Near { get; set; }
        public float Far { get; set; }
        public Vector3 Direction { get; set; }
        public Vector3 Position { get; set; } = new Vector3(0, 0, 0);
        public Vector3 Target { get; set; } = new Vector3(0, 0, 1);
        public Vector3 CameraUp { get; set; } = new Vector3(0, 1, 0);

        public Vector2 Viewport { get; set; }

        public Camera(Vector2 viewport)
        {

            Position = new Vector3(0f, -1f, -2f);
            Direction = Vector3.Normalize(Position - Target);

            Near = 0.1F;
            Far = 100F;
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
}
