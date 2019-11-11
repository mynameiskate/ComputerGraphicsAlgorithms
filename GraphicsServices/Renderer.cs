using GraphicsServices.GraphicObjTypes;
using GraphicsServices.RenderObjTypes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Windows;
using System.Windows.Media.Imaging;
using Camera = GraphicsServices.RenderObjTypes.Camera;
using Point = System.Drawing.Point;

namespace GraphicsServices
{
    public class Renderer
    {
        private byte[] backBuffer;
        private WriteableBitmap bmp;
        public Bgr24Bitmap bitmap { get; protected set; }

        public Renderer(WriteableBitmap bmp, Bgr24Bitmap bitmap)
        {
            this.bmp = bmp;
            this.bitmap = bitmap;
            // 4 stands for RGBA
            backBuffer = new byte[bmp.PixelWidth * bmp.PixelHeight * 4];
        }

        // Choosing only points that fit on screen
        public void DrawPoints(IEnumerable<Vector2> points)
        {
            var filteredPoints = points
                .Where(point => (point.X >= 0) && (point.X < bmp.PixelWidth)
                    && (point.Y >= 0) && (point.Y < bmp.PixelHeight))
                .Select((point) => new Point((int)point.X, (int)point.Y));
            DrawPixels(filteredPoints, Color.DarkOrange);
        }

        // Bresenham's algorithm
        public void DrawLine(Vector2 point0, Vector2 point1, List<PixelInfo> sidesList, ZBuffer zBuf)
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
            var bgColor = Color.DarkOliveGreen;
            System.Windows.Media.Color color = System.Windows.Media.Color.FromArgb(bgColor.A, bgColor.R, bgColor.G, bgColor.B);
            bmp.Clear(color);
        }

        // Writing a chunk of pixels to bitmap
        private void DrawPixels(IEnumerable<Point> points, Color color)
        {
            try
            {
                // Reserve the back buffer for updates.
                bmp.Lock();

                unsafe
                {
                    foreach(var point in points)
                    {
                        // Get a pointer to the back buffer.
                        IntPtr pBackBuffer = bmp.BackBuffer;
                        int column = point.X;
                        int row = point.Y;

                        // Find the address of the pixel to draw.
                        pBackBuffer += row * bmp.BackBufferStride;
                        pBackBuffer += column * 4;

                        // Compute the pixel's color.
                        int color_data = color.R << 16;
                        color_data |= color.G << 8;
                        color_data |= color.B << 0;
                        color_data |= 255 << 24;

                        // Assign the color data to the pixel.
                        *((int*)pBackBuffer) = color_data;

                        bmp.AddDirtyRect(new Int32Rect(column, row, 1, 1));
                    }
                }
            }
            finally
            {
                // Release the back buffer and make it available for display.
                bmp.Unlock();
            }
        }

        // Projecting 3D to 2D
        public Vector2 Project(Vector4 coord, Matrix4x4 transMat)
        {
            var point = Vector4.Transform(coord, transMat);

            point /= point.W;

            var x = point.X * (bmp.PixelWidth / 2.0f) + bmp.PixelWidth / 2.0f;
            var y = -point.Y * (bmp.PixelHeight / 2.0f) + bmp.PixelHeight / 2.0f;

            return (new Vector2(x, y));
        }

        // Transforming initial vertices with the help of world, view, projection matrices.
        // Note: vertices are drawn in groups (faces).
        public void Render(Camera camera, RenderObj[] meshes, AxisType axis)
        {
            ZBuffer zBuf = new ZBuffer(bitmap.PixelWidth, bitmap.PixelHeight);

            var centerVector = Vector3.UnitX;
            if (axis == AxisType.Y)
                centerVector = Vector3.UnitY;
            else if (axis == AxisType.Z)
                centerVector = Vector3.UnitZ;
 
            var viewMatrix = Matrix4x4.CreateLookAt(camera.Position, camera.Target, centerVector);
            var projectionMatrix = Matrix4x4.CreatePerspectiveFieldOfView((float)Math.PI /3,
                                       (float)bmp.PixelHeight / bmp.PixelWidth,
                                       1f, 2f);

            foreach (RenderObj mesh in meshes)
            {
                var worldMatrix = Matrix4x4.CreateScale(mesh.Scale) *
                                  Matrix4x4.CreateRotationY(mesh.Rotation.Y, mesh.Position) *
                                  Matrix4x4.CreateTranslation(mesh.Position);

                var transformMatrix = worldMatrix * viewMatrix * projectionMatrix;

                foreach (var face in mesh.Faces)
                {
                    //if (IsFaceVisible(face))
                    //{
                    var sidesList = new List<PixelInfo>();

                    Vector3 lightingVector = new Vector3(0, 0, -1);
                   /* Vector3 point1Normal = DrawingObject.normalList[(int)face[0].Z];
                    Vector3 point2Normal = DrawingObject.normalList[(int)face[1].Z];
                    Vector3 point3Normal = DrawingObject.normalList[(int)face[2].Z];
                    Color point1Color = LamberLighting.GetPointColor(point1Normal, lightingVector, ObjectColor);
                    Color point2Color = LamberLighting.GetPointColor(point2Normal, lightingVector, ObjectColor);
                    Color point3Color = LamberLighting.GetPointColor(point3Normal, lightingVector, ObjectColor);
                    faceColor = PlaneShading.GetAverageColor(point1Color, point2Color, point3Color);*/

                    var pixels = new Vector2[face.VertexIndexList.Length];

                        for (int i = 0; i < face.VertexIndexList.Length; i++)
                        {
                            pixels[i] = Project(mesh.Vertices[face.VertexIndexList[i] - 1], transformMatrix);
                        }

                        for (int i = 0; i < pixels.Length - 1; i++)
                        {
                            DrawLine(pixels[i], pixels[i + 1], sidesList, zBuf);
                        }

                        DrawLine(pixels[0], pixels[pixels.Length - 1], sidesList, zBuf);

                        Rasterization.DrawPixelForRasterization(sidesList, bitmap, zBuf, System.Windows.Media.Color.FromArgb(10, 10, 10, 255));
                    //}
                }
            }

            //GraphicObjTypes.Rasterization.DrawPixelForRasterization(sidesList, Bitmap, zBuf, faceColor);
        }

        private bool IsFaceVisible(List<Vector3> face)
        {
            bool result = true;

           /* var normal = GetFaceNormal(face);

            if (normal.Z >= 0)
            {
                result = false;
            }*/

            return result;
        }

        /*private Vector3 GetFaceNormal(List<Vector3> face)
        {
            int indexPoint1 = (int)face[0].X;
            int indexPoint2 = (int)face[1].X;
            int indexPoint3 = (int)face[2].X;
            Vector4 point1 = DrawingObject.pointsList[indexPoint1];
            Vector4 point2 = DrawingObject.pointsList[indexPoint2];
            Vector4 point3 = DrawingObject.pointsList[indexPoint3];

            Vector4 vector1 = point2 - point1;
            Vector4 vector2 = point3 - point1;
            Vector3 vector1XYZ = new Vector3(vector1.X, vector1.Y, vector1.Z);
            Vector3 vector2XYZ = new Vector3(vector2.X, vector2.Y, vector2.Z);

            return Vector3.Normalize(Vector3.Cross(vector1XYZ, vector2XYZ));
        }*/
    }
}