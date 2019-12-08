using System.Drawing;
using System.Numerics;
using ArgbColor = System.Windows.Media.Color;

namespace GraphicsServices.Extensions
{
    public static class ColorExtensions
    {
        public static ArgbColor ToMedia(this Color color)
        {
            return ArgbColor.FromArgb(color.A, color.R, color.G, color.B);
        }

        public static Vector3 ToVector3(this ArgbColor color)
        {
            return new Vector3(color.R, color.G, color.B);
        }

        public static ArgbColor ToColor(this Vector3 colorVector)
        {
            return ArgbColor.FromArgb(255, (byte)colorVector.X, (byte)colorVector.Y, (byte)colorVector.Z);
        }
    }
}
