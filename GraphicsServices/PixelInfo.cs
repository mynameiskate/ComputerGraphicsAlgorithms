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
        public Vector3 Vt;
        public float W;
        public Color Color;
        private Color defaultColor = DefColor.White.ToMedia();

        public PixelInfo()
        { }

        public PixelInfo(int x, int y, float z, float w, Vector3 vn, Vector3 vt, Color? color = null)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
            Vn = vn;
            Vt = vt;
            Color = color ?? defaultColor;
        }
    }
}
