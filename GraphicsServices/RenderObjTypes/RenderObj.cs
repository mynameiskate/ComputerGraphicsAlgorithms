using GraphicsServices.GraphicObjTypes;
using System.Numerics;

namespace GraphicsServices.RenderObjTypes
{
    public class RenderObj
    {
        public string Name { get; set; }
        public Vector3[] Vertices { get; private set; }
        public Face[] Faces { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public int Direction = 1;

        public RenderObj(string name, int verticesCount, int facesCount)
        {
            Name = name;
            Vertices = new Vector3[verticesCount];
            Faces = new Face[facesCount];
        }
    }
}
