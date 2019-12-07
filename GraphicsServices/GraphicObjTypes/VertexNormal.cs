using System;
using System.Globalization;
using System.Numerics;

namespace GraphicsServices.GraphicObjTypes
{
    public class VertexNormal : IGraphicObject
    {
        public string Prefix { get; } = GraphicTypes.VertexNormal;
        public int RequiredDataLength { get; } = 3;

        public double I { get; set; }
        public double J { get; set; }
        public double K { get; set; }

        public void ParseFromString(string[] data)
        {
            if (data.Length < RequiredDataLength)
                throw new ArgumentException($"Required data length for type vertex is {RequiredDataLength}");

            if (!data[0].ToLower().Equals(Prefix))
                throw new ArgumentException($"Required data prefix for type vertex is {Prefix}");

            bool success;

            double i, j, k;

            success = double.TryParse(data[1], NumberStyles.Any, CultureInfo.InvariantCulture, out i);
            if (!success) throw new ArgumentException($"Type vertex normal: Could not parse {data[1]} as I");

            success = double.TryParse(data[2], NumberStyles.Any, CultureInfo.InvariantCulture, out j);
            if (!success) throw new ArgumentException($"Type vertex: Could not parse {data[2]} as J");

            success = double.TryParse(data[3], NumberStyles.Any, CultureInfo.InvariantCulture, out k);
            if (!success) throw new ArgumentException($"Type vertex: Could not parse {data[3]} as K");

            I = i;
            J = j;
            K = k;
        }

        public Vector3 ToVector()
        {
            return new Vector3((float)I, (float)J, (float)K);
        }

        public override string ToString()
        {
            return $"{Prefix} {I} {J} {K}";
        }
    }
}
