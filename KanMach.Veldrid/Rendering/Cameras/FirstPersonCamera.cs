using KanMach.Veldrid.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

using Veldrid.Sdl2;

namespace KanMach.Veldrid.Graphics.Cameras
{
    public class FirstPersonCamera : Camera
    {
        private Vector2 _rotation;
        private Vector3 _position;

        public override Vector3 Position 
        { 
            get => _position;
            set {
                _position = value;
                UpdateRotation(Rotation);
            }
        }
        public override Vector3 Target { get; set; }
        public Vector2 Rotation
        {
            get => _rotation;
            set {
                UpdateRotation(value);
                _rotation = value;
            }
        }
        
        public FirstPersonCamera(RenderContext renderContext, Vector2 viewport) : base(renderContext, viewport)
        {
        }

        private void UpdateRotation(Vector2 rotation)
        {
            var forward = -Vector3.UnitZ;
            var transform = Matrix4x4.CreateFromYawPitchRoll(rotation.X, rotation.Y, 0);
            Target = Position + Vector3.Transform(forward, transform);
        }

    } 
}
