using System;
using System.Globalization;
using System.Numerics;

namespace GraphicsServices.GraphicObjTypes
{
    public class Vertex : IGraphicObject
    {
        public string Prefix { get; } = GraphicTypes.Vertex;
        public int RequiredDataLength { get; } = 4;

        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double? W { get; set; }

        public void ParseFromString(string[] data)
        {
            if (data.Length < RequiredDataLength)
                throw new ArgumentException($"Required data length for type vertex is {RequiredDataLength}");

            if (!data[0].ToLower().Equals(Prefix))
                throw new ArgumentException($"Required data prefix for type vertex is {Prefix}");

            bool success;

            double x, y, z, w;

            success = double.TryParse(data[1], NumberStyles.Any, CultureInfo.InvariantCulture, out x);
            if (!success) throw new ArgumentException($"Type vertex: Could not parse {data[1]} as X");

            success = double.TryParse(data[2], NumberStyles.Any, CultureInfo.InvariantCulture, out y);
            if (!success) throw new ArgumentException($"Type vertex: Could not parse {data[2]} as Y");

            success = double.TryParse(data[3], NumberStyles.Any, CultureInfo.InvariantCulture, out z);
            if (!success) throw new ArgumentException($"Type vertex: Could not parse {data[3]} as Z");

            X = x;
            Y = y;
            Z = z;

            if (data.Length > RequiredDataLength)
            {
                success = double.TryParse(data[4], NumberStyles.Any, CultureInfo.InvariantCulture, out w);
                if (!success) throw new ArgumentException($"Type vertex: Could not parse {data[4]} as W");
                W = w;
            }
            else
            {
                W = 1;
            }
        }

        public Vector3 ToVector()
        {
            return new Vector3((float)X, (float)Y, (float)Z);
        }

        public Vector4 ToVector4()
        {
            return new Vector4((float)X, (float)Y, (float)Z, (float)W);
        }

        public override string ToString()
        {
            return $"{Prefix} {X} {Y} {Z}" + W ?? $" {W}";
        }
    }
}
