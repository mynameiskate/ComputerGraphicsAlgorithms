using System.Numerics;

namespace GraphicsServices.Extensions
{
    public static class VectorExtensions
    {
        public static Vector3 ToVector3(this Vector4 vector)
        {
            return new Vector3(vector.X, vector.Y, vector.Z);
        }
    }
}
