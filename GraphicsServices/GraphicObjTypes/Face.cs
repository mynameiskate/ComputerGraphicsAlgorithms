using System;
using System.Globalization;
using System.Text;

namespace GraphicsServices.GraphicObjTypes
{
    public class Face : IGraphicObject
    {
        public string Prefix { get; } = GraphicTypes.Face;
        public int RequiredDataLength { get; } = 2;

        public int[] VertexIndexList { get; set; }
        public int[] TextureVertexIndexList { get; set; }
        public int[] VertexNormalIndexList { get; set; }

        public void ParseFromString(string[] data)
        {
            if (data.Length < RequiredDataLength)
                throw new ArgumentException($"Required data length for type vertex is {RequiredDataLength}");

            if (!data[0].ToLower().Equals(Prefix))
                throw new ArgumentException($"Required data prefix for type vertex is {Prefix}");

            int vertexCount = data.Length - 1;
            VertexIndexList = new int[vertexCount];
            TextureVertexIndexList = new int[vertexCount];
            VertexNormalIndexList = new int[vertexCount];

            bool success;

            for (int i = 0; i < vertexCount; i++)
            {
                string[] parts = data[i + 1].Split('/');

                int index;
                success = int.TryParse(parts[0], NumberStyles.Any, CultureInfo.InvariantCulture, out index);
                if (!success) throw new ArgumentException($"Type face: Could not parse parameter {parts[0]} as int");
                VertexIndexList[i] = index;

                if (parts.Length > 1)
                {
                    success = int.TryParse(parts[1], NumberStyles.Any, CultureInfo.InvariantCulture, out index);
                    if (success)
                    {
                        TextureVertexIndexList[i] = index;
                    }

                    if (parts.Length > 2)
                    {
                        success = int.TryParse(parts[2], NumberStyles.Any, CultureInfo.InvariantCulture, out index);
                        if (success)
                        {
                            VertexNormalIndexList[i] = index;
                        }
                    }
                }
            }
        }

        public override string ToString()
        {
            if (VertexIndexList.Length == 0)
            {
                return string.Empty;
            }

            StringBuilder sb = new StringBuilder();
            sb.Append(Prefix);

            for (int i = 0; i < VertexIndexList.Length; i++)
            {
                string faceStr = $" {VertexIndexList[i]}";

                string optionalStr = (i < VertexNormalIndexList.Length) ? $"/{VertexNormalIndexList[i]}" : "";

                if (i < TextureVertexIndexList.Length)
                {
                    faceStr += $"/{TextureVertexIndexList[i]}";
                }
                else if (optionalStr.Length > 0)
                {
                    faceStr += "/";
                }

                sb.Append(faceStr);
            }

            return sb.ToString();
        }
    }
}
