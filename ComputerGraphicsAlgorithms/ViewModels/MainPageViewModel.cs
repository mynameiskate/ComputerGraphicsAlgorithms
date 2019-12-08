using GraphicsServices.Lighting;
using System.ComponentModel;
using System.Numerics;

namespace ComputerGraphicsAlgorithms.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        #region Camera position
        private float _xCameraPos = 1f;
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

        private float _zCameraPos = 1f;
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
