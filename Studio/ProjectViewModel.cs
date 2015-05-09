namespace Studio
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows.Media.Imaging;
    using Annotations;
    using System.Windows.Controls;
    using System.Windows.Input;

    public class ProjectViewModel : INotifyPropertyChanged
    {
        private readonly Image imageViewControl;

        public Project Project { get; }
        

        public double ImageXRatio { get; private set; }
        public double ImageYRatio { get; private set; }

        public ICommand NomalizeCommand => new NormalizationCommand();
        public ICommand NomalizerToolCommand => new NormalizationToolCommand();

        private void CalculateRatio()
        {
            if (imageViewControl.Source == null)
            {
                return;
            }

            var imageSource = (BitmapSource)imageViewControl.Source;

            ImageXRatio =  imageViewControl.RenderSize.Width /(imageSource.PixelWidth);
            ImageYRatio =  imageViewControl.RenderSize.Height /(imageSource.PixelHeight);

            OnPropertyChanged("ImageXRatio");
            OnPropertyChanged("ImageYRatio");
        }

        

        

        public ProjectViewModel(string imageFile, Image imageViewControl)
        {
            Project = Project.Create(imageFile);
            this.imageViewControl = imageViewControl;
            
            imageViewControl.SizeChanged += (sender, args) =>
            {
                CalculateRatio();

                OnPropertyChanged("ImageXRatio");
                OnPropertyChanged("ImageYRatio");
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}