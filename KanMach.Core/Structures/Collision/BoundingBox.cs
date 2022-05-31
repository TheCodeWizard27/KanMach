using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace KanMach.Core.Structures.Collision
{
    public enum BBCollisionType
    {
        Disjoint,
        Contains,
        Intersects
    }

    // TODO Change to struct?
    public class BoundingBox
    {
        private Vector3 _min;
        private Vector3 _max;

        public Vector3 Min { get
            {
                return _min;
            }
            set
            {
                _min = value;
                InitCorners();
            }
        }
        public Vector3 Max
        {
            get
            {
                return _max;
            }
            set
            {
                _max = value;
                InitCorners();
            }
        }

        public Vector3 Center { get => (Min + Max) / 2f; }
        public Vector3 Dimensions { get => Max - Min; }

        public Vector3 NearTopLeft { get; private set; }
        public Vector3 NearTopRight { get; private set; }
        public Vector3 NearBottomLeft { get; private set; }
        public Vector3 NearBottomRight { get; private set; }

        public Vector3 FarTopLeft { get; private set; }
        public Vector3 FarTopRight { get; private set; }
        public Vector3 FarBottomLeft { get; private set; }
        public Vector3 FarBottomRight { get; private set; }

        public BoundingBox(Vector3 min, Vector3 max)
        {
            _min = min;
            _max = max;
            InitCorners();
        }

        public bool Contains(BoundingBox other)
        {
            return GetCollision(other) == BBCollisionType.Contains;
        }
        public bool Intersects(BoundingBox other)
        {
            return GetCollision(other) is BBCollisionType.Contains or BBCollisionType.Intersects;
        }

        public BBCollisionType GetCollision(BoundingBox other)
        {
            if (Max.X < other.Min.X || Min.X > other.Max.X
               || Max.Y < other.Min.Y || Min.Y > other.Max.Y
               || Max.Z < other.Min.Z || Min.Z > other.Max.Z)
            {
                return BBCollisionType.Disjoint;
            }
            else if (Min.X <= other.Min.X && Max.X >= other.Max.X
                && Min.Y <= other.Min.Y && Max.Y >= other.Max.Y
                && Min.Z <= other.Min.Z && Max.Z >= other.Max.Z)
            {
                return BBCollisionType.Contains;
            }
            else
            {
                return BBCollisionType.Intersects;
            }
        }

        private void InitCorners()
        {
            NearBottomLeft = new Vector3(Min.X, Min.Y, Max.Z);
            NearBottomRight = new Vector3(Max.X, Min.Y, Max.Z);
            NearTopLeft = new Vector3(Min.X, Max.Y, Max.Z);
            NearTopRight = _max;

            FarBottomLeft = _min;
            FarBottomRight = new Vector3(Max.X, Min.Y, Min.Z);
            FarTopLeft = new Vector3(Min.X, Max.Y, Min.Z);
            FarTopRight = new Vector3(Max.X, Max.Y, Min.Z);
        }

    }
}
