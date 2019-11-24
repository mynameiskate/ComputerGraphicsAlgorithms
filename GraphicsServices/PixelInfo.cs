using GraphicsServices.Extensions;
using System.Windows.Media;
using DefColor = System.Drawing.Color;

namespace GraphicsServices
{
    public class PixelInfo
    {
        public int X;
        public int Y;
        public float Z;
        public Color Color;
        private Color defaultColor = DefColor.White.ToMedia();

        public PixelInfo(int x, int y, float z, Color? color = null)
        {
            X = x;
            Y = y;
            Z = z;
            Color = color ?? defaultColor;
        }
    }
}
