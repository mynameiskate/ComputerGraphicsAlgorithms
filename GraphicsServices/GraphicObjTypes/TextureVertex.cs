using System;
using System.Globalization;
using System.Numerics;

namespace GraphicsServices.GraphicObjTypes
{
    public class TextureVertex : IGraphicObject
    {
        public string Prefix { get; } = GraphicTypes.TextureVertex;
        public int RequiredDataLength { get; } = 2;

        public double U { get; set; }
        public double? V { get; set; }
        public double? W { get; set; }

        public void ParseFromString(string[] data)
        {
            if (data.Length < RequiredDataLength)
                throw new ArgumentException($"Required data length for type vertex is {RequiredDataLength}");

            if (!data[0].ToLower().Equals(Prefix))
                throw new ArgumentException($"Required data prefix for type vertex is {Prefix}");

            bool success;

            double u, v, w;

            success = double.TryParse(data[1], NumberStyles.Any, CultureInfo.InvariantCulture, out u);
            if (!success) throw new ArgumentException($"Type texture vertex: Could not parse {data[1]} as U");
            U = u;

            if (data.Length > RequiredDataLength)
            {
                success = double.TryParse(data[2], NumberStyles.Any, CultureInfo.InvariantCulture, out v);
                if (!success) throw new ArgumentException($"Type texture vertex: Could not parse {data[2]} as V");
                V = v;

                if (data.Length > RequiredDataLength + 1)
                {
                    success = double.TryParse(data[3], NumberStyles.Any, CultureInfo.InvariantCulture, out w);
                    if (!success) throw new ArgumentException($"Type texture vertex: Could not parse {data[3]} as W");
                    W = w;
                }

            }
        }

        public Vector3 ToVector()
        {
            return new Vector3((float)U, (float)V, (float)W);
        }

        public override string ToString()
        {
            return $"{Prefix} {U}" + V ?? $" {V}" + W ?? $" {W}";
        }
    }
}
