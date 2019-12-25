using System;
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

        public static Vector3 ToByteVector3(this Vector3 vector)
        {
            var x = Math.Min(vector.X, 255);
            var y = Math.Min(vector.Y, 255);
            var z = Math.Min(vector.Z, 255);

            return new Vector3(x, y, z);
        }
    }
}
