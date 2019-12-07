using GraphicsServices.Lighting;
using System.ComponentModel;
using System.Numerics;

namespace ComputerGraphicsAlgorithms.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        #region Camera position
        private float _xPos = 1f;
        public float XPos
        {
            get { return _xPos; }
            set
            {
                _xPos = value;
                OnPropertyChanged(new PropertyChangedEventArgs("XPos"));
            }
        }

        private float _yPos = 1f;
        public float YPos
        {
            get { return _yPos; }
            set
            {
                _yPos = value;
                OnPropertyChanged(new PropertyChangedEventArgs("YPos"));
            }
        }

        private float _zPos = 1f;
        public float ZPos
        {
            get { return _zPos; }
            set
            {
                _zPos = value;
                OnPropertyChanged(new PropertyChangedEventArgs("ZPos"));
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
