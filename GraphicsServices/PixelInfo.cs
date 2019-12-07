using GraphicsServices.Extensions;
using System.Numerics;
using System.Windows.Media;
using DefColor = System.Drawing.Color;

namespace GraphicsServices
{
    public class PixelInfo
    {
        public int X;
        public int Y;
        public float Z;
        public Vector3 Vn;
        public Color Color;
        private Color defaultColor = DefColor.White.ToMedia();

        public PixelInfo()
        { }

        public PixelInfo(int x, int y, float z, Vector3 vn, Color? color = null)
        {
            X = x;
            Y = y;
            Z = z;
            Vn = vn;
            Color = color ?? defaultColor;
        }
    }
}
