using System;
using System.Numerics;
using BmpColor = System.Windows.Media.Color;

namespace GraphicsServices.Lighting
{
    public class BaseLighting
    {
        public Vector3 AmbientColor { get; set; }
        public Vector3 Vector { get; set; } = new Vector3(0);
        public float Intensity { get; set; } = 1;

        private BmpColor penColor = BmpColor.FromArgb(255, 0, 0, 0);

        public BaseLighting()
        {
        }

        public BaseLighting(BmpColor bgColor)
        {
            AmbientColor = new Vector3(bgColor.R, bgColor.G, bgColor.B);
        }

        public BaseLighting(BmpColor color, BmpColor bgColor)
        {
            penColor = color;
            AmbientColor = new Vector3(bgColor.R, bgColor.G, bgColor.B);
        }

        public virtual BmpColor GetColorForPoint(Vector3 normal)
        {
            var lightingVectorNormal = Vector3.Normalize(Vector);
            var mul = Vector3.Multiply(-lightingVectorNormal, normal);
            float k = (mul.X + mul.Y + mul.Z) * Intensity;

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

        public virtual BmpColor GetTexturizedColorForPoint(Vector3 vector31, Vector3 vector32)
        {
            throw new NotImplementedException();
        }
    }
}
