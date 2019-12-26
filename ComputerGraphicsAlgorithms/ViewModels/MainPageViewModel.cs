using GraphicsServices.Lighting;
using System.ComponentModel;
using System.Numerics;

namespace ComputerGraphicsAlgorithms.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        #region Camera position
        private float _xCameraPos = 0f;
        public float XCameraPos
        {
            get { return _xCameraPos; }
            set
            {
                _xCameraPos = value;
                OnPropertyChanged(new PropertyChangedEventArgs("XCameraPos"));
            }
        }

        private float _yCameraPos = 1f;
        public float YCameraPos
        {
            get { return _yCameraPos; }
            set
            {
                _yCameraPos = value;
                OnPropertyChanged(new PropertyChangedEventArgs("YCameraPos"));
            }
        }

        private float _zCameraPos = 4f;
        public float ZCameraPos
        {
            get { return _zCameraPos; }
            set
            {
                _zCameraPos = value;
                OnPropertyChanged(new PropertyChangedEventArgs("ZCameraPos"));
            }
        }
        #endregion

        #region Lighting parameters
        private float _xLightPos = -1f;
        public float XLightPos
        {
            get { return _xLightPos; }
            set
            {
                _xLightPos = value;
                OnPropertyChanged(new PropertyChangedEventArgs("XLightPos"));
            }
        }

        private float _yLightPos = 0f;
        public float YLightPos
        {
            get { return _yLightPos; }
            set
            {
                _yLightPos = value;
                OnPropertyChanged(new PropertyChangedEventArgs("YLightPos"));
            }
        }

        private float _zLightPos = 1f;
        public float ZLightPos
        {
            get { return _zLightPos; }
            set
            {
                _zLightPos = value;
                OnPropertyChanged(new PropertyChangedEventArgs("ZLightPos"));
            }
        }

        private float _intensity = 1f;
        public float Intensity
        {
            get { return _intensity; }
            set
            {
                _intensity = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Intensity"));
            }
        }

        private float _glossCoefficient = 50f;
        public float GlossCoefficient
        {
            get { return _glossCoefficient; }
            set
            {
                _glossCoefficient = value;
                OnPropertyChanged(new PropertyChangedEventArgs("GlossCoefficient"));
            }
        }

        private float _xKa = 0.1f;
        public float xKa
        {
            get { return _xKa; }
            set
            {
                _xKa = value;
                OnPropertyChanged(new PropertyChangedEventArgs("xKa"));
            }
        }

        private float _yKa = 0.1f;
        public float yKa
        {
            get { return _yKa; }
            set
            {
                _yKa = value;
                OnPropertyChanged(new PropertyChangedEventArgs("yKa"));
            }
        }

        private float _zKa = 0.1f;
        public float zKa
        {
            get { return _zKa; }
            set
            {
                _zKa = value;
                OnPropertyChanged(new PropertyChangedEventArgs("zKa"));
            }
        }

        private float _xKd = 1f;
        public float xKd
        {
            get { return _xKd; }
            set
            {
                _xKd = value;
                OnPropertyChanged(new PropertyChangedEventArgs("xKd"));
            }
        }

        private float _yKd = 1f;
        public float yKd
        {
            get { return _yKd; }
            set
            {
                _yKd = value;
                OnPropertyChanged(new PropertyChangedEventArgs("yKd"));
            }
        }

        private float _zKd = 1f;
        public float zKd
        {
            get { return _zKd; }
            set
            {
                _zKd = value;
                OnPropertyChanged(new PropertyChangedEventArgs("zKd"));
            }
        }

        private float _xKs = 0.7f;
        public float xKs
        {
            get { return _xKs; }
            set
            {
                _xKs = value;
                OnPropertyChanged(new PropertyChangedEventArgs("xKs"));
            }
        }

        private float _yKs = 0.7f;
        public float yKs
        {
            get { return _yKs; }
            set
            {
                _yKs = value;
                OnPropertyChanged(new PropertyChangedEventArgs("yKs"));
            }
        }

        private float _zKs = 0.7f;
        public float zKs
        {
            get { return _zKs; }
            set
            {
                _zKs = value;
                OnPropertyChanged(new PropertyChangedEventArgs("zKs"));
            }
        }
        #endregion

        #region Color parameters
        private float _xAmbientColor = 70f;
        public float XAmbientColor
        {
            get { return _xAmbientColor; }
            set
            {
                _xAmbientColor = value;
                OnPropertyChanged(new PropertyChangedEventArgs("XAmbientColor"));
            }
        }

        private float _yAmbientColor = 0f;
        public float YAmbientColor
        {
            get { return _yAmbientColor; }
            set
            {
                _yAmbientColor = value;
                OnPropertyChanged(new PropertyChangedEventArgs("YAmbientColor"));
            }
        }

        private float _zAmbientColor = 100f;
        public float ZAmbientColor
        {
            get { return _zAmbientColor; }
            set
            {
                _zAmbientColor = value;
                OnPropertyChanged(new PropertyChangedEventArgs("ZAmbientColor"));
            }
        }

        private float _xDiffuseColor = 0f;
        public float XDiffuseColor
        {
            get { return _xDiffuseColor; }
            set
            {
                _xDiffuseColor = value;
                OnPropertyChanged(new PropertyChangedEventArgs("XDiffuseColor"));
            }
        }

        private float _yDiffuseColor = 0f;
        public float YDiffuseColor
        {
            get { return _yDiffuseColor; }
            set
            {
                _yDiffuseColor = value;
                OnPropertyChanged(new PropertyChangedEventArgs("YDiffuseColor"));
            }
        }

        private float _zDiffuseColor = 255f;
        public float ZDiffuseColor
        {
            get { return _zDiffuseColor; }
            set
            {
                _zDiffuseColor = value;
                OnPropertyChanged(new PropertyChangedEventArgs("ZDiffuseColor"));
            }
        }

        private float _xSpecularColor = 255f;
        public float XSpecularColor
        {
            get { return _xSpecularColor; }
            set
            {
                _xSpecularColor = value;
                OnPropertyChanged(new PropertyChangedEventArgs("XSpecularColor"));
            }
        }

        private float _ySpecularColor = 255f;
        public float YSpecularColor
        {
            get { return _ySpecularColor; }
            set
            {
                _ySpecularColor = value;
                OnPropertyChanged(new PropertyChangedEventArgs("YSpecularColor"));
            }
        }

        private float _zSpecularColor = 255f;
        public float ZSpecularColor
        {
            get { return _zSpecularColor; }
            set
            {
                _zSpecularColor = value;
                OnPropertyChanged(new PropertyChangedEventArgs("ZSpecularColor"));
            }
        }
        #endregion

        #region Object parameters
        private int _xObjectPos = 0;
        public int XObjectPos
        {
            get { return _xObjectPos; }
            set
            {
                _xObjectPos = value;
                OnPropertyChanged(new PropertyChangedEventArgs("XObjectPos"));
            }
        }

        private int _yObjectPos = 0;
        public int YObjectPos
        {
            get { return _yObjectPos; }
            set
            {
                _yObjectPos = value;
                OnPropertyChanged(new PropertyChangedEventArgs("YObjectPos"));
            }
        }

        private int _zObjectPos = 0;
        public int ZObjectPos
        {
            get { return _zObjectPos; }
            set
            {
                _zObjectPos = value;
                OnPropertyChanged(new PropertyChangedEventArgs("ZObjectPos"));
            }
        }

        private int _scale = 1;
        public int Scale
        {
            get { return _scale; }
            set
            {
                _scale = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Scale"));
            }
        }
        #endregion

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }
        #endregion
    }
}
