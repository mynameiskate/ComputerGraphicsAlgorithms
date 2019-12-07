using System.Numerics;
using BmpColor = System.Windows.Media.Color;

namespace GraphicsServices.Lighting
{
    public class BaseLighting
    {
        public Vector3 vector = new Vector3(0, 0, 0);
        public float intensity = 1;

        public BmpColor GetColorForPoint(Vector3 normal, BmpColor penColor)
        {
            var lightingVectorNormal = Vector3.Normalize(vector);
            var mul = Vector3.Multiply(-lightingVectorNormal, normal);
            float k = (mul.X + mul.Y + mul.Z) * intensity;

            if (k >= 0 && k <= 1)
            {
                float r = penColor.R * k;
                float g = penColor.G * k;
                float b = penColor.B * k;

                return BmpColor.FromArgb(255, (byte)r, (byte)g, (byte)b);
            }
            else if (k < 0 || double.IsNaN(k))
            {
                return BmpColor.FromArgb(255, 0, 0, 0);
            }
            else
            {
                return penColor;
            }
        }
    }
}
