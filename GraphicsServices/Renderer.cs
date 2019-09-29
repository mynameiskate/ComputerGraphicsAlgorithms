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

        public Renderer(WriteableBitmap bmp)
        {
            this.bmp = bmp;
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
        public void DrawLine(Vector2 point0, Vector2 point1)
        {
            int x0 = (int)point0.X;
            int y0 = (int)point0.Y;
            int x1 = (int)point1.X;
            int y1 = (int)point1.Y;

            var dx = Math.Abs(x1 - x0);
            var dy = Math.Abs(y1 - y0);
            var sx = (x0 < x1) ? 1 : -1;
            var sy = (y0 < y1) ? 1 : -1;
            var err = dx - dy;

            var drawnPoints = new List<Vector2>();

            while ((x0 != x1) && (y0 != y1))
            {
                drawnPoints.Add(new Vector2(x0, y0));

                var e2 = 2 * err;
                if (e2 > -dy) { err -= dy; x0 += sx; }
                if (e2 < dx) { err += dx; y0 += sy; }
            }

            DrawPoints(drawnPoints);
        }


        public void Clear()
        {
            bmp.Clear();
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
        public Vector2 Project(Vector3 coord, Matrix4x4 transMat)
        {
            var point = Vector3.Transform(coord, transMat);

            var x = point.X + bmp.PixelWidth / 2.0f;
            var y = -point.Y + bmp.PixelHeight * 0.9f;
            return (new Vector2(x, y));
        }

        // Transforming initial vertices with the help of world, view, projection matrices.
        // Note: vertices are drawn in groups (faces).
        public void Render(Camera camera, RenderObj[] meshes)
        {
            var viewMatrix = Matrix4x4.CreateLookAt(camera.Position, camera.Target, Vector3.UnitY);
            var projectionMatrix = Matrix4x4.CreatePerspectiveFieldOfView(0.9f,
                                                           (float)bmp.PixelWidth / bmp.PixelHeight,
                                                           10.0f, 20.0f);
            foreach (RenderObj mesh in meshes)
            {
                var worldMatrix = Matrix4x4.CreateRotationY(mesh.Rotation.Y, mesh.Position) *
                                  Matrix4x4.CreateTranslation(mesh.Position) *
                                  Matrix4x4.CreateScale(new Vector3(3, 2, 1), mesh.Position);

                var transformMatrix = worldMatrix * viewMatrix * projectionMatrix;

                foreach (var face in mesh.Faces)
                {
                    var vertexA = mesh.Vertices[face.VertexIndexList[0] - 1];
                    var vertexB = mesh.Vertices[face.VertexIndexList[1] - 1];
                    var vertexC = mesh.Vertices[face.VertexIndexList[2] - 1];

                    var pixelA = Project(vertexA, transformMatrix);
                    var pixelB = Project(vertexB, transformMatrix);
                    var pixelC = Project(vertexC, transformMatrix);

                    DrawLine(pixelA, pixelB);
                    DrawLine(pixelB, pixelC);
                    DrawLine(pixelC, pixelA);
                }
            }
        }
    }
}