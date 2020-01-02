using GraphicsServices.GraphicObjTypes;
using GraphicsServices.RenderObjTypes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Windows.Media.Imaging;
using GraphicsServices.Extensions;
using System.Threading.Tasks;
using GraphicsServices.Lighting;
using Camera = GraphicsServices.RenderObjTypes.Camera;
using Point = System.Drawing.Point;
using BmpColor = System.Windows.Media.Color;

namespace GraphicsServices
{
    public class Renderer
    {
        private byte[] backBuffer;
        private BmpColor bgColor = Color.LightSlateGray.ToMedia();
        public Bgr24Bitmap bmp { get; protected set; }
        private ZBuffer zBuf;
        private BaseLighting lighting;

        public Renderer(WriteableBitmap baseBitmap, BaseLighting lighting)
        {
            bmp = new Bgr24Bitmap(baseBitmap);
            zBuf = new ZBuffer(baseBitmap.PixelWidth, baseBitmap.PixelHeight);
            // 4 stands for RGBA
            backBuffer = new byte[baseBitmap.PixelWidth * baseBitmap.PixelHeight * 4];
            this.lighting = lighting;

            if (lighting.AmbientColor != null)
            {
                bgColor = lighting.AmbientColor.ToColor();
            }
            else
            {
                lighting.AmbientColor = bgColor.ToVector3();
            }
        }

        public void Clear()
        {
            bmp.Clear(bgColor);
        }

        // Bresenham's algorithm
        public List<PixelInfo> GetSides(RenderObj obj, PixelInfo point0, PixelInfo point1)
        {
            int x0 = point0.X;
            int y0 = point0.Y;
            float z0 = point0.Z;
            int x1 = point1.X;
            int y1 = point1.Y;
            float z1 = point1.Z;
            float w0 = 1 / point0.W;
            float w1 = 1 / point1.W;
            Vector3 vn0 = point0.Vn * w0;
            Vector3 vn1 = point1.Vn * w1;
            Vector3 vt0 = point0.Vt * w0;
            Vector3 vt1 = point1.Vt * w1;

            int deltaX = Math.Abs(x1 - x0);
            int deltaY = Math.Abs(y1 - y0);
            int error = deltaX - deltaY;

            var sidePoints = new List<PixelInfo>();
            sidePoints.Add(new PixelInfo(x1, y1, z1, w1, vn1, vt1));

            if (UpdateZBuffer(x1, y1, z1))
            {
                if (lighting is PhongTexturizingLighting)
                {
                    bmp[x1, y1] = lighting.GetTexturizedColorForPoint(obj, vn1 / w1, vt1/ w1);
                }
                else
                {
                    bmp[x1, y1] = lighting.GetColorForPoint(vn1 / w1);
                }
            }

            // In this case there is no point in further actions
            if (deltaX == 0 && deltaY == 0)
            {
                return sidePoints;
            }

            int delta = (deltaY == 0) ? deltaX : deltaY;
            float deltaZ = Math.Abs(z1 - z0) / delta;
            float deltaW = (w1 - w0) / delta;
            Vector3 deltaVn = (vn1 - vn0) / delta;
            Vector3 deltaVt = (vt1 - vt0) / delta;

            int signX = x0 < x1 ? 1 : -1;
            int signY = y0 < y1 ? 1 : -1;
            float signZ = z0 < z1 ? 1 : -1;

            while (x0 != x1 || y0 != y1)
            {
                sidePoints.Add(new PixelInfo(x0, y0, z0, w0, vn0, vt0));

                int error2 = error * 2;

                if (error2 > -deltaY)
                {
                    error -= deltaY;
                    x0 += signX;
                }

                if (error2 < deltaX)
                {
                    if (UpdateZBuffer(x0, y0, z0))
                    {
                        if (lighting is PhongTexturizingLighting)
                        {
                            bmp[x0, y0] = lighting.GetTexturizedColorForPoint(obj, vn0 / w0, vt0 / w0);
                        }
                        else
                        {
                            bmp[x0, y0] = lighting.GetColorForPoint(vn0 / w0);
                        }
                    }

                    error += deltaX;
                    y0 += signY;
                    z0 += signZ * deltaZ;
                    w0 += deltaW;
                    vn0 += deltaVn;
                    vt0 += deltaVt;
                }
            }

            return sidePoints;
        }

        // Projecting 3D to 2D
        private Vector4 Project(Vector4 coord, Matrix4x4 transMat)
        {
            var point = Vector4.Transform(coord, transMat);
            var w = point.W;
            point /= w;

            var x = point.X * (bmp.PixelWidth / 2.0f) + bmp.PixelWidth / 2.0f;
            var y = -point.Y * (bmp.PixelHeight / 2.0f) + bmp.PixelHeight / 2.0f;

            return (new Vector4(x, y, point.Z, w));
        }

        // Transforming initial vertices with the help of world, view, projection matrices.
        // Note: vertices are drawn in groups (faces).
        public void Render(Camera camera, RenderObj[] meshes, AxisType axis)
        {
            var centerVector = axis.ToVector3();
            var viewMatrix = Matrix4x4.CreateLookAt(camera.Position, camera.Target, centerVector);
            var projectionMatrix = Matrix4x4.CreatePerspectiveFieldOfView((float)Math.PI / 6,
                                       (float)bmp.PixelWidth / bmp.PixelHeight,
                                       1f, 2f);

            foreach (RenderObj mesh in meshes)
            {
                var worldMatrix = Matrix4x4.CreateScale(mesh.Scale) *
                                  Matrix4x4.CreateRotationY(mesh.Rotation.Y, mesh.Position) *
                                  Matrix4x4.CreateTranslation(mesh.Position);
                mesh.ModelMatrix = worldMatrix;
                var transformMatrix = worldMatrix * viewMatrix * projectionMatrix;

                Parallel.ForEach(mesh.Faces, (face) =>
                {
                    var pixels = new Vector4[face.VertexIndexList.Length];
                    var normals = new Vector3[face.VertexNormalIndexList.Length];
                    var textures = new Vector3[face.TextureVertexIndexList.Length];

                    for (int i = 0; i < face.VertexIndexList.Length; i++)
                    {
                        pixels[i] = Project(mesh.Vertices[face.VertexIndexList[i] - 1], transformMatrix);
                    }

                    for (int i = 0; i < face.VertexNormalIndexList.Length; i++)
                    {
                        normals[i] = Vector3.Normalize(Vector3.TransformNormal(mesh.Normals[face.VertexNormalIndexList[i] - 1], worldMatrix));
                    }

                    for (int i = 0; i < face.TextureVertexIndexList.Length; i++)
                    {
                        textures[i] = Vector3.TransformNormal(mesh.TextureCoordinates[face.TextureVertexIndexList[i] - 1],
                            Matrix4x4.CreateRotationY(mesh.Rotation.Y, mesh.Position) * Matrix4x4.CreateTranslation(mesh.Position));
                    }


                    var normal = GetSurfaceNormal(
                        new Vector3(pixels[0].X, pixels[0].Y, pixels[0].Z),
                        new Vector3(pixels[1].X, pixels[1].Y, pixels[1].Z),
                        new Vector3(pixels[2].X, pixels[2].Y, pixels[2].Z)
                    );

                    if (normal.Z < 0)
                    {
                        var point1 = mesh.Vertices[face.VertexIndexList[0] - 1].ToVector3();
                        var point2 = mesh.Vertices[face.VertexIndexList[1] - 1].ToVector3();
                        var point3 = mesh.Vertices[face.VertexIndexList[2] - 1].ToVector3();

                        var sidePoints = new List<PixelInfo>();

                        for (int i = 0; i < pixels.Length - 1; i++)
                        {
                            sidePoints.AddRange(GetSides(
                                mesh,
                                new PixelInfo { X = (int)pixels[i].X, Y = (int)pixels[i].Y, Z = pixels[i].Z, Vn = normals[i], Vt = textures[i], W = pixels[i].W },
                                new PixelInfo { X = (int)pixels[i + 1].X, Y = (int)pixels[i + 1].Y, Z = pixels[i + 1].Z, Vn = normals[i + 1], Vt = textures[i + 1], W = pixels[i + 1].W }
                            ));
                        }

                        sidePoints.AddRange(GetSides(
                            mesh,
                            new PixelInfo { X = (int)pixels[0].X, Y = (int)pixels[0].Y, Z = pixels[0].Z, Vn = normals[0], Vt = textures[0], W = pixels[0].W },
                            new PixelInfo { X = (int)pixels[pixels.Length - 1].X, Y = (int)pixels[pixels.Length - 1].Y, Z = pixels[pixels.Length - 1].Z,
                                Vn = normals[pixels.Length - 1], Vt = textures[pixels.Length - 1], W = pixels[pixels.Length - 1].W
                            }
                        ));

                        Rasterize(mesh, sidePoints);
                    }
                });
            }
        }

        private void Rasterize(RenderObj obj, List<PixelInfo> sidePoints)
        {
            var yCoordsCount = sidePoints.Select(x => x.Y).Distinct().Count();

            if (yCoordsCount < 2)
            {
                // Swap x and y for the case when all pixels have the same Y
                // in order for interpolation to work
                foreach (var pixel in sidePoints)
                {
                    var temp = pixel.X;
                    pixel.X = pixel.Y;
                    pixel.Y = temp;
                }
            }

            RasterizeByY(obj, sidePoints);
        }

        private void RasterizeByY(RenderObj obj, List<PixelInfo> sidePoints)
        {
            var yList = sidePoints.OrderBy(x => x.Y).ToList();

            foreach (var pixel in yList)
            {
                int y = pixel.Y;

                var coordinates = sidePoints
                   .Where(coord => coord.Y == y)
                   .OrderBy(coord => coord.X)
                   .DistinctBy(coord => coord.X)
                   .ToList();

                var startPoint = coordinates.First();
                var endPoint = coordinates.Last();

                int signZ = startPoint.Z < endPoint.Z ? 1 : -1;
                var delta = endPoint.X - startPoint.X;
                float z = startPoint.Z;
                float w = startPoint.W;
                Vector3 vn = startPoint.Vn;
                Vector3 vt = startPoint.Vt;

                if (delta == 0) continue;

                float deltaZ = Math.Abs((endPoint.Z - startPoint.Z) / delta);
                float deltaW = (endPoint.W - startPoint.W) / delta;
                Vector3 deltaVn = (endPoint.Vn - startPoint.Vn) / delta;
                Vector3 deltaVt = (endPoint.Vt - startPoint.Vt) / delta;

                for (int x = startPoint.X; x < endPoint.X; x++)
                {
                    z += signZ * deltaZ;
                    vn += deltaVn;
                    vt += deltaVt;
                    w += deltaW;

                    if (UpdateZBuffer(x, y, z))
                    {
                        if (lighting is PhongTexturizingLighting)
                        {
                            bmp[x, y] = lighting.GetTexturizedColorForPoint(obj, vn / w, vt / w);
                        }
                        else
                        {
                            bmp[x, y] = lighting.GetColorForPoint(vn / w);
                        }
                    }
                }
            }
        }

        // Update Z-buffer if necessary
        private bool UpdateZBuffer(int x, int y, float z)
        {
            if (x > 0 && x < zBuf.Width && y > 0 && y < zBuf.Height)
            {
                if (z <= zBuf[x, y])
                {
                    zBuf[x, y] = z;

                    return true;
                }
            }

            return false;
        }

        // Writing a chunk of pixels to bitmap
        private void DrawPixels(IEnumerable<Point> points, BmpColor color)
        {
            try
            {
                bmp.Source.Lock();

                foreach (var point in points)
                {
                    bmp[point.X, point.Y] = color;
                }
            }
            finally
            {
                bmp.Source.Unlock();
            }
        }

        private Vector3 GetSurfaceNormal(Vector3 a, Vector3 b, Vector3 c)
        {
            var ab = Vector3.Subtract(b, a);
            var ac = Vector3.Subtract(c, a);
            var dir = Vector3.Cross(ab, ac);

            return Vector3.Normalize(dir);
        }
    }
}