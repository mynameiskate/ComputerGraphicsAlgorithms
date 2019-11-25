using GraphicsServices;
using GraphicsServices.GraphicObjTypes;
using GraphicsServices.RenderObjTypes;
using System;
using System.IO;
using System.Numerics;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Camera = GraphicsServices.RenderObjTypes.Camera;
using Lighting = GraphicsServices.RenderObjTypes.Lighting;

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
        Lighting lighting = new Lighting();

        // Temporary example of file name for parsing
        string path = "african_head.obj";
        private int _scale = 0;
        private float _xPos = 1f;
        private float _yPos = 1f;
        private float _zPos = 1f;
        private AxisType axis = AxisType.X;
        private int dpiX;
        private int dpiY;

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
            GetDPI();
        }

        private void LoadFile(string fileName)
        {
            var parser = new ObjParser();
            var path = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\", "examples", fileName));
            parser.LoadObj(path);

            camera.Position = new Vector3(_xPos, _yPos, _zPos);
            camera.Target = Vector3.Zero;

            mesh = new RenderObj(parser.VertexList.Count, parser.FaceList.Count);
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
            WriteableBitmap bmp = new WriteableBitmap((int)image.Width, (int)image.Height,
                dpiX, dpiY, PixelFormats.Bgra32, null);
            renderer = new Renderer(bmp, lighting);

            //mesh.Rotation = new Vector3(mesh.Rotation.X, mesh.Rotation.Y - 0.01f, mesh.Rotation.Z);
            mesh.Scale = _scale;
            camera.Position = new Vector3(_xPos, _yPos, _zPos);

            /*if (mesh.Direction < 0)
            {
                if (z <= -range) { mesh.Direction = 1; } else { z--; };
            }
            else
            {
                if (z >= range) { mesh.Direction = -1; } else { z++; };
            }*/

            /*mesh.Position = new Vector3(mesh.Position.X, mesh.Position.Y, z);*/
            renderer.Clear();
            renderer.Render(camera, new RenderObj[] { mesh }, axis);
            image.Source = renderer.bmp.Source;
        }

        private void GetDPI()
        {
            var dpiXProperty = typeof(SystemParameters).GetProperty("DpiX", BindingFlags.NonPublic | BindingFlags.Static);
            var dpiYProperty = typeof(SystemParameters).GetProperty("Dpi", BindingFlags.NonPublic | BindingFlags.Static);

            dpiX = (int)dpiXProperty.GetValue(null, null);
            dpiY = (int)dpiYProperty.GetValue(null, null);
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

            if (zAxisRadioBtn != null)
            {
                xAxisRadioBtn.IsChecked = false;
                zAxisRadioBtn.IsChecked = false;
            }
        }

        private void ZAxisRadioBtn_Checked(object sender, RoutedEventArgs e)
        {
            axis = AxisType.Z;
            xAxisRadioBtn.IsChecked = false;
            yAxisRadioBtn.IsChecked = false;
        }

        private void xLightPos_Changed(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var value = xLightPosUpdown.Value;

            if (value != null)
            {
                lighting.vector.X = (float)value;
            }
        }

        private void yLightPos_Changed(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var value = yLightPosUpdown.Value;

            if (value != null)
            {
                lighting.vector.Y = (float)value;
            }
        }

        private void zLightPos_Changed(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var value = zLightPosUpdown.Value;

            if (value != null)
            {
                lighting.vector.Z = (float)value;
            }
        }

        private void lightIntensity_Changed(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var value = lightIntensityUpdown.Value;

            if (value != null)
            {
                lighting.intensity = (float)value;
            }
        }
    }
}
