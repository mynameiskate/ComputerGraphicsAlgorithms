using System;
using System.Numerics;
using BmpColor = System.Windows.Media.Color;

namespace GraphicsServices.Lighting
{
    public class PhongLighting : BaseLighting
    {
        public Vector3 ViewVector { get; set; }

        #region Background lighting parameters
        public Vector3 Ka { get; set; }

        // 𝐼𝑎 = 𝑘𝑎 ∙ 𝑖𝑎
        private Vector3 BackgroundLightVector
        {
            get
            {
                return Ka * AmbientColor;
            }
        }
        #endregion

        #region Diffused lighting parameters
        public Vector3 DiffuseColor { get; set; }
        public Vector3 Kd { get; set; }

        // 𝐼𝑑 = 𝑘𝑑 ∙ (𝑁 ∙ 𝐿) ∙ 𝑖𝑑
        private Vector3 GetDiffusedLightVector(Vector3 normal)
        {
            return Kd * Math.Max(Vector3.Dot(normal, Vector), 0) * DiffuseColor;
        }
        #endregion

        #region Mirror lighting parameters
        public Vector3 SpecularColor { get; set; }
        public Vector3 Ks { get; set; }
        public float GlossCoefficient { get; set; }

        // 𝐼𝑠 = 𝑘𝑠 ∙ (𝑅 ∙ 𝑉)∝ ∙ 𝑖𝑠
        private Vector3 GetSpecularLightVector(Vector3 normal)
        {
            // 𝑅 = 𝐿 − 2 ∙ (𝐿 ∙ 𝑁) ∙ 𝑁
            var reflectionVector = Vector3.Normalize(Vector3.Reflect(Vector, normal));
            return Ks * (float)Math.Pow(Math.Max(Vector3.Dot(reflectionVector, new Vector3(0, 0, -1)), 0), GlossCoefficient) * SpecularColor;
        }
        #endregion

        private Vector3 GetPhongLightVector(Vector3 normal)
        {
            var resultVector = BackgroundLightVector
                + GetDiffusedLightVector(normal)
                + GetSpecularLightVector(normal);

            resultVector.X = Math.Min(resultVector.X, 255);
            resultVector.Y = Math.Min(resultVector.Y, 255);
            resultVector.Z = Math.Min(resultVector.Z, 255);

            return resultVector;
        }

        public override BmpColor GetColorForPoint(Vector3 normal)
        {
            var lightVector = GetPhongLightVector(normal);
            return BmpColor.FromArgb(255, (byte)lightVector.X, (byte)lightVector.Y, (byte)lightVector.Z);
        }
    }
}
