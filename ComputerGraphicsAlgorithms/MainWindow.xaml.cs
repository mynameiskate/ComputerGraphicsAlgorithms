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
        string path = "diablo3_pose.obj";
        private int _scale = 0;
        private float _xPos = 1f;
        private float _yPos = 1f;
        private float _zPos = 1f;
        private AxisType axis = AxisType.X;

        public int CurrentScale
        {
            get { return _scale; }
            set
            {
                _scale = value;

            }
        }

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
                image.Width, image.Height, PixelFormats.Bgra32, null);

            renderer = new Renderer(bmp);
            camera.Position = new Vector3(_xPos, _yPos, _zPos);
            camera.Target = Vector3.Zero;
            image.Source = bmp;

            mesh = new RenderObj("Fox", parser.VertexList.Count, parser.FaceList.Count);
            for (var i = 0; i < parser.VertexList.Count; i++)
            {
                mesh.Vertices[i] = parser.VertexList[i].ToVector4();
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
            mesh.Scale = _scale;
            camera.Position = new Vector3(_xPos, _yPos, _zPos);

            /*var z = mesh.Position.Z;
            var range = 10;*/

            /*if (mesh.Direction < 0)
            {
                if (z <= -range) { mesh.Direction = 1; } else { z--; };
            }
            else
            {
                if (z >= range) { mesh.Direction = -1; } else { z++; };
            }*/

            //if ((mesh.Scale < 1) || (mesh.Scale <= 2))
            //{
            //    mesh.Scale++;
            //}
            //else if (mesh.Scale > 5)
            //{
            //    mesh.Scale--;
            //}

            /*mesh.Position = new Vector3(mesh.Position.X, mesh.Position.Y, z);*/
            renderer.Clear();
            renderer.Render(camera, new RenderObj[] { mesh }, axis);
        }

        private void ScaleChanged(Object sender, EventArgs e)
        {
            var value = myUpDownControl.Value;

            if (value != null)
            {
                _scale = (int)myUpDownControl.Value;
            }
        }

        private void xCameraPos_Changed(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var value = xPosUpdown.Value;

            if (value != null)
            {
                _xPos = (int)value;
            }
        }

        private void yCameraPos_Changed(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var value = yPosUpdown.Value;

            if (value != null)
            {
                _yPos = (int)value;
            }
        }

        private void zCameraPos_Changed(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var value = zPosUpdown.Value;

            if (value != null)
            {
                _zPos = (int)value;
            }
        }

        private void XAxisRadioBtn_Checked(object sender, RoutedEventArgs e)
        {
            axis = AxisType.X;

            if (yAxisRadioBtn != null)
            {
                yAxisRadioBtn.IsChecked = false;
                zAxisRadioBtn.IsChecked = false;
            }
        }

        private void YAxisRadioBtn_Checked(object sender, RoutedEventArgs e)
        {
            axis = AxisType.Y;
            xAxisRadioBtn.IsChecked = false;
            zAxisRadioBtn.IsChecked = false;
        }

        private void ZAxisRadioBtn_Checked(object sender, RoutedEventArgs e)
        {
            axis = AxisType.Z;
            xAxisRadioBtn.IsChecked = false;
            yAxisRadioBtn.IsChecked = false;
        }
    }
}
