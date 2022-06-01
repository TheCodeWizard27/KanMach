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

    public struct BoundingBox : IEquatable<BoundingBox>
    {
        public Vector3 Min;
        public Vector3 Max;

        public Vector3 Center { get => (Min + Max) / 2f; }
        public Vector3 Dimensions { get => Max - Min; }

        public Vector3 NearBottomLeft { get => new Vector3(Min.X, Min.Y, Max.Z); }
        public Vector3 NearBottomRight { get => new Vector3(Max.X, Min.Y, Max.Z); }
        public Vector3 NearTopLeft { get => new Vector3(Min.X, Max.Y, Max.Z); }
        public Vector3 NearTopRight { get => Max; }
        public Vector3 FarBottomLeft { get => Min; }
        public Vector3 FarBottomRight { get => new Vector3(Max.X, Min.Y, Min.Z); }
        public Vector3 FarTopLeft { get => new Vector3(Min.X, Max.Y, Min.Z); }
        public Vector3 FarTopRight { get => new Vector3(Max.X, Max.Y, Min.Z); }

        public BoundingBox(Vector3 min, Vector3 max)
        {
            Min = min;
            Max = max;
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

        public static bool operator !=(BoundingBox first, BoundingBox second)
        {
            return !first.Equals(second);
        }

        public static bool operator ==(BoundingBox first, BoundingBox second)
        {
            return first.Equals(second);
        }

        public bool Equals(BoundingBox other)
        {
            return Min == other.Min && Max == other.Max;
        }
    }
}
