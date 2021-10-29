using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace KanMach.Veldrid.Graphics
{
    class MachCamera
    {
        public float Fov;
        public float Near;
        public float Far;
        public int Width;
        public int Height;

        public Vector3 Direction;
        public Vector3 Position;
        public Vector3 Target = new Vector3(0, 0, 0);
        public Vector3 CameraUp;
        public Vector3 CameraRight;

        public MachCamera(MachWindow mWindow)
        {
            Position = new Vector3(0f, -1f, -2f);
            Direction = Vector3.Normalize(Position - Target);

            Near = 0.1F;
            Far = 100F;
            Fov = 1f;

            Height = mWindow.Height;
            Width = mWindow.Width;

            var up = new Vector3(0, 1, 0);
            CameraRight = Vector3.Cross(up, Direction);
            CameraUp = Vector3.Cross(Direction, CameraRight);

        }
    } 
}
