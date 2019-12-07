﻿using System;
using System.Numerics;
using BmpColor = System.Windows.Media.Color;

namespace GraphicsServices.RenderObjTypes
{
    public class Lighting
    {
        public Vector3 vector = new Vector3(0, 0, 0);
        public float intensity = 1;

        public BmpColor GetNecessaryColor(Vector3 normal, BmpColor penColor)
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

        private static BmpColor GetAverageColor(BmpColor color1, BmpColor color2, BmpColor color3)
        {
            int sumR = color1.R + color2.R + color3.R;
            int sumG = color1.G + color2.G + color3.G;
            int sumB = color1.B + color2.B + color3.B;
            int sumA = color1.A + color2.A + color3.A;
            byte r = (byte)Math.Round((double)sumR / 3);
            byte g = (byte)Math.Round((double)sumG / 3);
            byte b = (byte)Math.Round((double)sumB / 3);
            byte a = (byte)Math.Round((double)sumA / 3);

            return BmpColor.FromArgb(a, r, g, b);
        }
    }
}