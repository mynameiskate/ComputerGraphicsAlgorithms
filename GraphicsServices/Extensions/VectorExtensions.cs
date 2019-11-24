using System.Numerics;

namespace GraphicsServices.Extensions
{
    public static class VectorExtensions
    {
        public static Vector3 ToVector3(this Vector4 vector)
        {
            return new Vector3(vector.X, vector.Y, vector.Z);
        }

        public static Vector3 ToVector3(this AxisType axisType)
        {
            switch(axisType)
            {
                case AxisType.X:
                    return Vector3.UnitX;
                case AxisType.Y:
                    return Vector3.UnitY;
                default:
                    return Vector3.UnitZ;
            }
        }
    }
}
