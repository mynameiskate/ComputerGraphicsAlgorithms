﻿using ComputerGraphicsAlgorithms.ViewModels;
using GraphicsServices;
using GraphicsServices.Lighting;
using GraphicsServices.RenderObjTypes;
using System;
using System.ComponentModel;
using System.IO;
using System.Numerics;
using System.Reflection;
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
        MainPageViewModel vm;
        Renderer renderer;
        Camera camera = new Camera();
        RenderObj mesh;
        PhongLighting lighting = new PhongTexturizingLighting //PhongLighting lighting = new PhongLighting
        {
            AmbientColor = new Vector3(0, 0, 0),
            DiffuseColor = new Vector3(0, 0, 0),
            SpecularColor = new Vector3(0, 0, 0),
            GlossCoefficient = 0.0f,
            Ka = new Vector3(0.0f),
            Kd = new Vector3(0.0f),
            Ks = new Vector3(0.0f)
        };

        // Temporary example of file name for parsing
        string path = "african_head";
       // string path = "diablo3_pose";
        private bool isTexturing = false;
        private AxisType axis = AxisType.X;
        private int dpiX;
        private int dpiY;

        public MainWindow()
        {
            InitializeComponent();
            vm = new MainPageViewModel();
            DataContext = vm;
            vm.PropertyChanged += view_PropertyChanged;
            GetDPI();
            UpdateLighting();
        }

        private void LoadFile(string fileName)
        {
            var objPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\", "examples", $"{fileName}.{FileExtensions.Object}"));
            var normalsPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\", "examples", $"{fileName}_{FileExtensions.Normal}.{FileExtensions.ImgType}"));
            var diffuseTexturePath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\", "examples", $"{fileName}_{FileExtensions.Diffuse}.{FileExtensions.ImgType}"));
            var specularTexturePath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\", "examples", $"{fileName}_{FileExtensions.Specular}.{FileExtensions.ImgType}"));

            var parser = new ObjParser();
            parser.LoadObj(objPath);

            camera.Position = new Vector3(vm.XCameraPos, vm.YCameraPos, vm.ZCameraPos);
            camera.Target = Vector3.Zero;

            mesh = new RenderObj(parser.VertexList.Count, parser.FaceList.Count, parser.NormalList.Count, parser.TextureList.Count);
            mesh.NormalTexture = parser.LoadTexture(normalsPath);
            mesh.DiffuseTexture = parser.LoadTexture(diffuseTexturePath);
            mesh.SpecularTexture = parser.LoadTexture(specularTexturePath);
            mesh.Position = new Vector3(vm.XObjectPos, vm.YObjectPos, vm.ZObjectPos);

            for (var i = 0; i < parser.VertexList.Count; i++)
            {
                mesh.Vertices[i] = parser.VertexList[i].ToVector4();
            }

            mesh.Faces = parser.FaceList.ToArray();

            for (var i = 0; i < parser.NormalList.Count; i++)
            {
                mesh.Normals[i] = parser.NormalList[i].ToVector();
            }

            for (var i = 0; i < parser.TextureList.Count; i++)
            {
                mesh.TextureCoordinates[i] = parser.TextureList[i].ToVector();
            }

            UpdateAnimation();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadFile(path);
        }

        private void UpdateLighting()
        {
            var ambientColor = lighting.AmbientColor;
            var diffuseColor = lighting.DiffuseColor;
            var specularColor = lighting.SpecularColor;
            var glossCoefficient = lighting.GlossCoefficient;
            var Ka = lighting.Ka;
            var Kd = lighting.Kd;
            var Ks = lighting.Ks;

            if (isTexturing)
            {
                lighting = new PhongTexturizingLighting
                {
                    AmbientColor = ambientColor,
                    DiffuseColor = diffuseColor,
                    SpecularColor = specularColor,
                    GlossCoefficient = glossCoefficient,
                    Ka = Ka,
                    Kd = Kd,
                    Ks = Ks
                };
            }
            else
            {
                lighting = new PhongLighting
                {
                    AmbientColor = ambientColor,
                    DiffuseColor = diffuseColor,
                    SpecularColor = specularColor,
                    GlossCoefficient = glossCoefficient,
                    Ka = Ka,
                    Kd = Kd,
                    Ks = Ks
                };
            }
        }

        private void UpdateAnimation()
        {
            if (mesh == null)
            {
                return;
            }

            WriteableBitmap bmp = new WriteableBitmap((int)image.Width, (int)image.Height,
                dpiX, dpiY, PixelFormats.Bgra32, null);
            lighting.GlossCoefficient = vm.GlossCoefficient;
            lighting.Ka = new Vector3(vm.xKa, vm.yKa, vm.zKa);
            lighting.Kd = new Vector3(vm.xKd, vm.yKd, vm.zKd);
            lighting.Ks = new Vector3(vm.xKs, vm.yKs, vm.zKs);
            lighting.AmbientColor = new Vector3(vm.XAmbientColor, vm.YAmbientColor, vm.ZAmbientColor);
            lighting.DiffuseColor = new Vector3(vm.XDiffuseColor, vm.YDiffuseColor, vm.ZDiffuseColor);
            lighting.SpecularColor = new Vector3(vm.XSpecularColor, vm.YSpecularColor, vm.ZSpecularColor);
            lighting.Vector = Vector3.Normalize(new Vector3(vm.XLightPos, vm.YLightPos, vm.ZLightPos));
            renderer = new Renderer(bmp, lighting);
            mesh.Scale = vm.Scale;
            mesh.Position = new Vector3(vm.XObjectPos, vm.YObjectPos, vm.ZObjectPos);
            camera.Position = new Vector3(vm.XCameraPos, vm.YCameraPos, vm.ZCameraPos);
            lighting.ViewVector = camera.Position;

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

        private void view_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateAnimation();
        }

        private void GetDPI()
        {
            var dpiXProperty = typeof(SystemParameters).GetProperty("DpiX", BindingFlags.NonPublic | BindingFlags.Static);
            var dpiYProperty = typeof(SystemParameters).GetProperty("Dpi", BindingFlags.NonPublic | BindingFlags.Static);

            dpiX = (int)dpiXProperty.GetValue(null, null);
            dpiY = (int)dpiYProperty.GetValue(null, null);
        }

        private void XAxisRadioBtn_Checked(object sender, RoutedEventArgs e)
        {
            axis = AxisType.X;

            if (yAxisRadioBtn != null)
            {
                yAxisRadioBtn.IsChecked = false;
                zAxisRadioBtn.IsChecked = false;
                UpdateAnimation();
            }
        }

        private void YAxisRadioBtn_Checked(object sender, RoutedEventArgs e)
        {
            axis = AxisType.Y;

            if (zAxisRadioBtn != null)
            {
                xAxisRadioBtn.IsChecked = false;
                zAxisRadioBtn.IsChecked = false;
                UpdateAnimation();
            }
        }

        private void ZAxisRadioBtn_Checked(object sender, RoutedEventArgs e)
        {
            axis = AxisType.Z;
            xAxisRadioBtn.IsChecked = false;
            yAxisRadioBtn.IsChecked = false;
            UpdateAnimation();
        }

        private void ShadingRadioBtn_Checked(object sender, RoutedEventArgs e)
        {
            isTexturing = false;

            if (texturingRadioBtn != null)
            {
                texturingRadioBtn.IsChecked = false;
                UpdateLighting();
                UpdateAnimation();
            }
        }

        private void TexturingRadioBtn_Checked(object sender, RoutedEventArgs e)
        {
            isTexturing = true;
            shadingRadioBtn.IsChecked = false;
            UpdateLighting();
            UpdateAnimation();
        }
    }
}
