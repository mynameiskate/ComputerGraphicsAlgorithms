using System;
using System.Drawing;
using System.Numerics;

namespace GraphicsServices.Lighting
{
    public class PhongLighting : BaseLighting
    {
        public Vector3 LightVector { get; set; }
        public Vector3 ViewVector { get; set; }

        #region Background lighting parameters
        public Vector3 AmbientColor { get; set; }
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
            return Kd * Math.Max(Vector3.Dot(normal, LightVector), 0) * DiffuseColor;
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
            var reflectionVector = Vector3.Normalize(Vector3.Reflect(LightVector, normal));
            return Ks * (float)Math.Pow(Math.Max(Vector3.Dot(reflectionVector, ViewVector), 0), GlossCoefficient) * SpecularColor;
        }
        #endregion

        private Vector3 GetPhongLightVector(Vector3 normal)
        {
            return BackgroundLightVector
                + GetDiffusedLightVector(normal)
                + GetSpecularLightVector(normal);
        }

        public Color GetColorForPoint(Vector3 normal)
        {
            var lighVector = GetPhongLightVector(normal);
            return Color.FromArgb(255, (byte)lighVector.X, (byte)lighVector.Y, (byte)lighVector.Z);
        }
    }
}
