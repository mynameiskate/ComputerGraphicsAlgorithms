using GraphicsServices.GraphicObjTypes;
using System;
using System.Collections.Generic;
using System.IO;

namespace GraphicsServices
{
    public class ObjParser
    {
        public List<Vertex> VertexList;
        public List<Face> FaceList;
        public List<TextureVertex> TextureList;
        public List<VertexNormal> NormalList;

	    public ObjParser()
        {
            VertexList = new List<Vertex>();
            FaceList = new List<Face>();
            TextureList = new List<TextureVertex>();
            NormalList = new List<VertexNormal>();
        }

        public void LoadObj(string path)
        {
            var data = File.ReadAllLines(path);

            foreach (var line in data)
            {
                ParseLine(line);
            }
        }

        private void ParseLine(string line)
        {
            string[] parts = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length > 0)
            {
                switch (parts[0])
                {
                    case GraphicTypes.Vertex:
                        var v = new Vertex();
                        v.ParseFromString(parts);
                        VertexList.Add(v);
                        break;
                    case GraphicTypes.VertexNormal:
                        var vn = new VertexNormal();
                        vn.ParseFromString(parts);
                        NormalList.Add(vn);
                        break;
                    case GraphicTypes.Face:
                        var f = new Face();
                        f.ParseFromString(parts);
                        FaceList.Add(f);
                        break;
                    case GraphicTypes.TextureVertex:
                        var vt = new TextureVertex();
                        vt.ParseFromString(parts);
                        TextureList.Add(vt);
                        break;
                }
            }
        }
    }
}
