using GraphicsServices.Extensions;
using GraphicsServices.GraphicObjTypes;
using GraphicsServices.RenderObjTypes;
using System;
using System.Numerics;
using BmpColor = System.Windows.Media.Color;

namespace GraphicsServices.Lighting
{
    public class PhongTexturizingLighting : PhongLighting
    {
        public override BmpColor GetTexturizedColorForPoint(RenderObj obj, Vector3 normal, Vector3 texel)
        {
            return GetPhongTexturizedLightVector(obj, normal, texel).ToColor();
        }

        private Vector3 GetPhongTexturizedLightVector(RenderObj obj, Vector3 pointNormal, Vector3 texel)
        {
            var x = texel.X * obj.DiffuseTexture.PixelWidth;
            var y = (1 - texel.Y) * obj.DiffuseTexture.PixelHeight;

            if (x < 0 || y < 0 || x >= obj.DiffuseTexture.PixelWidth || y >= obj.DiffuseTexture.PixelHeight)
            {
                return new Vector3(0);
            }

            var normal = GetNormalTextureVector(obj.NormalTexture, x, y, obj.ModelMatrix) ?? pointNormal;

            var resultVector = GetAmbientTextureVector(obj.DiffuseTexture, x, y)
                + GetDiffusedTextureVector(obj.DiffuseTexture, x, y, normal)
                + GetSpecularTextureVector(obj.SpecularTexture, x, y, normal);

            return resultVector.ToByteVector3();
        }

        private Vector3? GetNormalTextureVector(Bgr24Bitmap texture, float x, float y, Matrix4x4 modelMatrix)
        {
            if (texture == null) return null;
            var normal = Interpolate(texture, x, y);
            //  𝑁 = Ct * 2 - 1
            normal -= new Vector3(byte.MaxValue) / 2;
            normal = Vector3.Normalize(normal);

            return Vector3.Normalize(Vector3.TransformNormal(normal, modelMatrix));
        }

        private Vector3 GetAmbientTextureVector(Bgr24Bitmap texture, float x, float y)
        {
            if (texture == null) return new Vector3(0);
            var textureVector = Interpolate(texture, x, y);

            return Vector3.Normalize(AmbientColor) * textureVector;
        }

        private Vector3 GetDiffusedTextureVector(Bgr24Bitmap texture, float x, float y, Vector3 normal)
        {
            if (texture == null) return new Vector3(0);
            var textureVector = Interpolate(texture, x, y);

            return Vector3.Normalize(DiffuseColor) * Math.Max(Vector3.Dot(normal, Vector), 0) * textureVector;
        }

        private Vector3 GetSpecularTextureVector(Bgr24Bitmap texture, float x, float y, Vector3 normal)
        {
            if (texture == null) return new Vector3(0);
            var reflectionVector = Vector3.Normalize(Vector3.Reflect(Vector, normal));
            var textureVector = Interpolate(texture, x, y);

            // 𝑅 = 𝐿 − 2 ∙ (𝐿 ∙ 𝑁) ∙ 𝑁
            return Vector3.Normalize(SpecularColor) * (float)Math.Pow(Math.Max(Vector3.Dot(reflectionVector, new Vector3(0, 0, -1)), 0), GlossCoefficient) * textureVector;
        }

        // Bilinear interpolation
        // z = (A(1 - x) + Bx)(1 - y) + (C(1 - x) + Dx)y
        // x = deltaX
        // y = deltaY
        private Vector3 Interpolate(Bgr24Bitmap texture, float x, float y)
        {
            int x1 = (int)x;
            int y1 = (int)y;

            float deltaX = x - x1;
            float deltaY = y - y1;

            var y0 = (1 - deltaX) * texture.GetVector(x1, y1) + deltaX * texture.GetVector(x1 + 1, y1);
            var y2 = (1 - deltaX) * texture.GetVector(x1, y1 + 1) + deltaX * texture.GetVector(x1 + 1, y1 + 1);
            return (1 - deltaX) * y0 + deltaX * y2;
        }
    }
}
