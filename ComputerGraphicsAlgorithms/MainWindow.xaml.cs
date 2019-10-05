﻿using GraphicsServices;
using GraphicsServices.RenderObjTypes;
using System;
using System.IO;
using System.Numerics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Camera = GraphicsServices.RenderObjTypes.Camera;

namespace ComputerGraphicsAlgorithms
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Renderer renderer;
        Camera camera = new Camera();
        RenderObj mesh;

        // Temporary example of file name for parsing
        string path = "african_head.obj";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoadFile(string fileName)
        {
            var parser = new ObjParser();
            var path = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\", "examples", fileName));
            parser.LoadObj(path);

            // Choose the back buffer resolution here
            WriteableBitmap bmp = new WriteableBitmap((int)image.Width, (int)image.Height,
                300, 300, PixelFormats.Bgra32, null);
            renderer = new Renderer(bmp);
            camera.Position = new Vector3(0, 0f, 40.0f);
            camera.Target = Vector3.Zero;
            image.Source = bmp;

            mesh = new RenderObj("Fox", parser.VertexList.Count, parser.FaceList.Count);
            for (var i = 0; i < parser.VertexList.Count; i++)
            {
                mesh.Vertices[i] = parser.VertexList[i].ToVector();
            }

            mesh.Faces = parser.FaceList.ToArray();

            CompositionTarget.Rendering += UpdateAnimation;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadFile(path);
        }

        private void UpdateAnimation(object sender, object e)
        {
            mesh.Rotation = new Vector3(mesh.Rotation.X, mesh.Rotation.Y - 0.01f, mesh.Rotation.Z);

            var z = mesh.Position.Z;
            var range = 30;

            if (mesh.Direction < 0)
            {
                if (z <= -range) { mesh.Direction = 1; } else { z--; };
            }
            else
            {
                if (z >= range) { mesh.Direction = -1; } else { z++; };
            }

            //if ((mesh.Scale < 1) || (mesh.Scale <= 2))
            //{
            //    mesh.Scale++;
            //}
            //else if (mesh.Scale > 5)
            //{
            //    mesh.Scale--;
            //}

            mesh.Position = new Vector3(mesh.Position.X, mesh.Position.Y, z);
            renderer.Clear();
            renderer.Render(camera, new RenderObj[] { mesh });
        }
    }
}
