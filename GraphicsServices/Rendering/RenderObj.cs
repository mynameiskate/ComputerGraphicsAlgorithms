using GraphicsServices.GraphicObjTypes;
using System.Numerics;

namespace GraphicsServices.RenderObjTypes
{
    public class RenderObj
    {
        public Vector4[] Vertices { get; private set; }
        public Face[] Faces { get; set; }
        public Vector3[] Normals { get; private set; }
        public Vector3[] TextureCoordinates { get; private set; }
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }

        public int Direction = 1;

        public int Scale = 1;
        public Matrix4x4 ModelMatrix { get; set; }
        public Bgr24Bitmap NormalTexture { get; set; }
        public Bgr24Bitmap DiffuseTexture { get; set; }
        public Bgr24Bitmap SpecularTexture { get; set; }

        public RenderObj(int verticesCount, int facesCount, int normalsCount, int texturesCount)
        {
            Vertices = new Vector4[verticesCount];
            Faces = new Face[facesCount];
            Normals = new Vector3[normalsCount];
            TextureCoordinates = new Vector3[texturesCount];
        }
    }
}
