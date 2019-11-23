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

namespace GraphicsServices
{
    public class Renderer
    {
        private byte[] backBuffer;
        private Color penColor = Color.DarkOrange;
        private Color bgColor = Color.DarkOliveGreen;
        public Bgr24Bitmap bmp { get; protected set; }

        public Renderer(WriteableBitmap baseBitmap)
        {
            bmp = new Bgr24Bitmap(baseBitmap);

            // 4 stands for RGBA
            backBuffer = new byte[baseBitmap.PixelWidth * baseBitmap.PixelHeight * 4];
        }

        // Choosing only points that fit on screen
        public void DrawPoints(IEnumerable<Vector2> points)
        {
            var filteredPoints = points
                .Where(point => (point.X >= 0) && (point.X < bmp.PixelWidth)
                    && (point.Y >= 0) && (point.Y < bmp.PixelHeight))
                .Select((point) => new Point((int)point.X, (int)point.Y));
            DrawPixels(filteredPoints, penColor.ToMedia());
        }

        // Bresenham's algorithm
        public void DrawLine(Vector4 point0, Vector4 point1, List<PixelInfo> sidesList, ZBuffer zBuf)
        {
            int x0 = (int)point0.X;
            int y0 = (int)point0.Y;
            int x1 = (int)point1.X;
            int y1 = (int)point1.Y;

            var drawnPoints = new List<Vector2>();

            x0 = x0 < 0 ? 0 : x0;
            x1 = x1 < 0 ? 0 : x1;
            y0 = y0 < 0 ? 0 : y0;
            y1 = y1 < 0 ? 0 : y1;

            int deltaX = Math.Abs(x1 - x0);
            int deltaY = Math.Abs(y1 - y0);
            int signX = x0 < x1 ? 1 : -1;
            int signY = y0 < y1 ? 1 : -1;

            int error = deltaX - deltaY;

            drawnPoints.Add(new Vector2(x1, y1));

            while (x0 != x1 || y0 != y1)
            {
                drawnPoints.Add(new Vector2(x0, y0));

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
                }
            }

            DrawPoints(drawnPoints);
        }


        public void Clear()
        {
            bmp.Clear(bgColor.ToMedia());
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

        // Projecting 3D to 2D
        public Vector4 Project(Vector4 coord, Matrix4x4 transMat)
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
            ZBuffer zBuf = new ZBuffer(bmp.PixelWidth, bmp.PixelHeight);

            var centerVector = Vector3.UnitX;
            if (axis == AxisType.Y)
                centerVector = Vector3.UnitY;
            else if (axis == AxisType.Z)
                centerVector = Vector3.UnitZ;
 
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

                // Parallel.ForEach(mesh.Faces, (face) =>
                foreach(var face in mesh.Faces)
                {
                    //var point1 = mesh.Vertices[face.VertexIndexList[0] - 1].ToVector3();
                    //var point2 = mesh.Vertices[face.VertexIndexList[1] - 1].ToVector3();
                    //var point3 = mesh.Vertices[face.VertexIndexList[0] - 1].ToVector3();

                    //var normal = GetSurfaceNormal(point1, point2, point3);

                    //if (normal.Z >= 0)
                    {
                        var sidesList = new List<PixelInfo>();

                        Vector3 lightingVector = new Vector3(0, 0, -1);

                        var pixels = new Vector4[face.VertexIndexList.Length];

                        for (int i = 0; i < face.VertexIndexList.Length; i++)
                        {
                            pixels[i] = Project(mesh.Vertices[face.VertexIndexList[i] - 1], transformMatrix);
                        }

                        for (int i = 0; i < pixels.Length - 1; i++)
                        {
                            DrawLine(pixels[i], pixels[i + 1], sidesList, zBuf);
                        }

                        DrawLine(pixels[0], pixels[pixels.Length - 1], sidesList, zBuf);
                    }

                    // Rasterization.DrawPixelForRasterization(sidesList, bmp, zBuf, BmpColor.FromArgb(10, 10, 10, 255));
                    //}
                }
               // );
                
            }

            //GraphicObjTypes.Rasterization.DrawPixelForRasterization(sidesList, Bitmap, zBuf, faceColor);
        }

        public Vector3 GetSurfaceNormal(Vector3 a, Vector3 b, Vector3 c)
        {
            var dir = Vector3.Cross(b - a, c - a);
            return Vector3.Normalize(dir);
        }
    }
}