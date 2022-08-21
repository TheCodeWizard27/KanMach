using System.Numerics;

namespace KanMach.Veldrid.Rendering.Structures
{
    public struct MaterialProperties
    {

        public Vector3 Diffuse = Vector3.One;
        public float Shininess = 32;
        public Vector3 Specular = new Vector3(0.5f);
        public float _padding = 0;

        public MaterialProperties()
        {

        }

    }
}
