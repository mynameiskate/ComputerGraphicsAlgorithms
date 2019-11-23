using System.Drawing;
using ArgbColor = System.Windows.Media.Color;

namespace GraphicsServices.Extensions
{
    public static class ColorExtensions
    {
        public static ArgbColor ToMedia(this Color color)
        {
            return ArgbColor.FromArgb(color.A, color.R, color.G, color.B);
        }
    }
}
