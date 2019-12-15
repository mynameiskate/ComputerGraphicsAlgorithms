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
        private float _xLightPos = 1f;
        public float XLightPos
        {
            get { return _xLightPos; }
            set
            {
                _xLightPos = value;
                OnPropertyChanged(new PropertyChangedEventArgs("XLightPos"));
            }
        }

        private float _yLightPos = 1f;
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

        private float _glossCoefficient = 0.6f;
        public float GlossCoefficient
        {
            get { return _glossCoefficient; }
            set
            {
                _glossCoefficient = value;
                OnPropertyChanged(new PropertyChangedEventArgs("GlossCoefficient"));
            }
        }

        private float _Ka = 0.1f;
        public float Ka
        {
            get { return _Ka; }
            set
            {
                _Ka = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Ka"));
            }
        }

        private float _Kd = 0.1f;
        public float Kd
        {
            get { return _Kd; }
            set
            {
                _Kd = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Kd"));
            }
        }

        private float _Ks = 0.1f;
        public float Ks
        {
            get { return _Ks; }
            set
            {
                _Ks = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Ks"));
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
