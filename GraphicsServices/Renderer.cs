using GraphicsServices.GraphicObjTypes;
using GraphicsServices.RenderObjTypes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Windows.Media.Imaging;
using GraphicsServices.Extensions;
using Camera = GraphicsServices.RenderObjTypes.Camera;
using Point = System.Drawing.Point;
using BmpColor = System.Windows.Media.Color;
using System.Threading.Tasks;
using GraphicsServices.Lighting;

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
        }

        public void Clear()
        {
            bmp.Clear(bgColor);
        }

        // Bresenham's algorithm
        public List<PixelInfo> GetSides(PixelInfo point0, PixelInfo point1, ZBuffer zBuf)
        {
            int x0 = point0.X;
            int y0 = point0.Y;
            float z0 = point0.Z;
            int x1 = point1.X;
            int y1 = point1.Y;
            float z1 = point0.Z;

            x0 = x0 < 0 ? 0 : x0;
            x1 = x1 < 0 ? 0 : x1;
            y0 = y0 < 0 ? 0 : y0;
            y1 = y1 < 0 ? 0 : y1;

            int deltaX = Math.Abs(x1 - x0);
            int deltaY = Math.Abs(y1 - y0);
            float deltaZ = Math.Abs(z1 - z0);
            Vector3 deltaVn = (point1.Vn - point0.Vn) / deltaY;
            int signX = x0 < x1 ? 1 : -1;
            int signY = y0 < y1 ? 1 : -1;
            float signZ = z0 < z1 ? 1 : -1;

            float curZ = point0.Z;
            float z3 = deltaZ / deltaY;
            Vector3 curVn = point0.Vn;

            int error = deltaX - deltaY;

            var sidePoints = new List<PixelInfo>();
            sidePoints.Add(new PixelInfo(x1, y1, z1, point1.Vn));

            if (UpdateZBuffer(x1, y1, curZ))
            {
                bmp[x1, y1] = lighting.GetColorForPoint(point1.Vn);
            }

            while (x0 != x1 || y0 != y1)
            {
                sidePoints.Add(new PixelInfo(x0, y0, z0, curVn));

                if (UpdateZBuffer(x0, y0, curZ))
                {
                    bmp[x0, y0] = lighting.GetColorForPoint(point0.Vn);
                }

                int error2 = error * 2;

                if (error2 > -deltaY)
                {
                    error -= deltaY;
                    x0 += signX;
                }
                if (error2 < deltaX)
                {
                    error += deltaX;
                    y0 += signY;
                    curZ += signZ * z3;
                    curVn += deltaVn;
                }
            }

            return sidePoints;
        }

        // Projecting 3D to 2D
        private Vector4 Project(Vector4 coord, Matrix4x4 transMat)
        {
            var point = Vector4.Transform(coord, transMat);

            point /= point.W;

            var x = point.X * (bmp.PixelWidth / 2.0f) + bmp.PixelWidth / 2.0f;
            var y = -point.Y * (bmp.PixelHeight / 2.0f) + bmp.PixelHeight / 2.0f;

            return (new Vector4(x, y, point.Z, point.W));
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

                var transformMatrix = worldMatrix * viewMatrix * projectionMatrix;

                Parallel.ForEach(mesh.Faces, (face) =>
                {
                    var pixels = new Vector4[face.VertexIndexList.Length];
                    var normals = new Vector3[face.VertexNormalIndexList.Length];

                    for (int i = 0; i < face.VertexIndexList.Length; i++)
                    {
                        pixels[i] = Project(mesh.Vertices[face.VertexIndexList[i] - 1], transformMatrix);
                    }

                    for (int i = 0; i < face.VertexNormalIndexList.Length; i++)
                    {
                        normals[i] = Vector3.Normalize(Vector3.TransformNormal(mesh.Normals[face.VertexNormalIndexList[i] - 1], worldMatrix));
                    }

                    var normal = GetSurfaceNormal(
                        new Vector3(pixels[0].X, pixels[0].Y, pixels[0].Z),
                        new Vector3(pixels[1].X, pixels[1].Y, pixels[1].Z),
                        new Vector3(pixels[2].X, pixels[2].Y, pixels[2].Z));

                    if (normal.Z < 0)
                    {
                        var point1 = mesh.Vertices[face.VertexIndexList[0] - 1].ToVector3();
                        var point2 = mesh.Vertices[face.VertexIndexList[1] - 1].ToVector3();
                        var point3 = mesh.Vertices[face.VertexIndexList[2] - 1].ToVector3();

                        // var localNormal = GetSurfaceNormal(point1, point2, point3);
                        // var color = lighting.GetColorForPoint(localNormal);
                        var sidePoints = new List<PixelInfo>();

                        for (int i = 0; i < pixels.Length - 1; i++)
                        {
                            sidePoints.AddRange(GetSides(
                                new PixelInfo { X = (int)pixels[i].X, Y = (int)pixels[i].Y, Z = pixels[i].Z, Vn = normals[i]},
                                new PixelInfo { X = (int)pixels[i + 1].X, Y = (int)pixels[i + 1].Y, Z = pixels[i + 1].Z, Vn = normals[i + 1] },
                                zBuf
                            ));
                        }

                        sidePoints.AddRange(GetSides(
                            new PixelInfo { X = (int)pixels[0].X, Y = (int)pixels[0].Y, Z = pixels[0].Z, Vn = normals[0] },
                            new PixelInfo { X = (int)pixels[pixels.Length - 1].X, Y = (int)pixels[pixels.Length - 1].Y, Z = pixels[pixels.Length - 1].Z, Vn = normals[pixels.Length - 1] },
                            zBuf
                        ));

                        Rasterize(sidePoints);
                    }
                });

            }
        }

        private void Rasterize(List<PixelInfo> sidePoints)
        {
            int minY, maxY;
            PixelInfo xStartPixel, xEndPixel;
            GetMinMaxY(sidePoints, out minY, out maxY);

            for (int y = minY + 1; y < maxY; y++)
            {
                FindStartAndEndXByY(sidePoints, y, out xStartPixel, out xEndPixel);
                int signZ = xStartPixel.Z < xEndPixel.Z ? 1 : -1;
                float deltaZ = Math.Abs((xEndPixel.Z - xStartPixel.Z) / (xEndPixel.X - xStartPixel.X));
                float curZ = xStartPixel.Z;
                Vector3 deltaVn = (xEndPixel.Vn - xStartPixel.Vn) / (xEndPixel.X - xStartPixel.X);
                Vector3 curVn = xStartPixel.Vn;

                for (int x = xStartPixel.X + 1; x < xEndPixel.X; x++)
                {
                    curZ += signZ * deltaZ;
                    curVn += deltaVn;

                    if (UpdateZBuffer(x, y, curZ))
                    {
                        bmp[x, y] = lighting.GetColorForPoint(curVn);
                    }
                }
            }
        }

        private void GetMinMaxY(List<PixelInfo> sidePoints, out int min, out int max)
        {
            var list = sidePoints.OrderBy(x => x.Y).ToList();
            min = list[0].Y;
            max = list[sidePoints.Count - 1].Y;
        }

        private void FindStartAndEndXByY(List<PixelInfo> sidePoints, int y, out PixelInfo xStartPixel, out PixelInfo xEndPixel)
        {
            // Ordering by x
            var sameYList = sidePoints
                .Where(x => x.Y == y)
                .OrderBy(x => x.X)
                .ToList();

            xStartPixel = sameYList[0];
            xEndPixel = sameYList[sameYList.Count - 1];
            for (int i = 1; i < sameYList.Count; i++)
            {
                if (sameYList[i].X - sameYList[i - 1].X > 1)
                {
                    xEndPixel = sameYList[i];
                    break;
                }
                else
                {
                    xStartPixel = sameYList[i];
                }
            }
        }

        // Update Z-buffer if necessary
        private bool UpdateZBuffer(int x, int y, float z)
        {
            if (x > 0 && x < zBuf.Width && y > 0 && y < zBuf.Height)
            {
                if (z <= zBuf[x, y]) // && z > 0 && z < 1
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