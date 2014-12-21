namespace Studio
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Windows.Media.Imaging;
    using Annotations;
    using Image = System.Windows.Controls.Image;

    public class ProjectViewModel : INotifyPropertyChanged
    {
        private readonly Image container;

        public string ImageFile
        {
            get { return imageFile; }
            set
            {
                imageFile = value;
                OnPropertyChanged();
                CalculateRatio();
                OnPropertyChanged("OriginalFeatures");
            }
        }

        private readonly Project project;
        private string imageFile;
        public List<FacialFeature> OriginalFeatures { get; }

        public double ImageXRatio { get; private set; }

        private void CalculateRatio()
        {
            if (container.Source == null)
            {
                return;
            }

            var imageSource = (BitmapSource)container.Source;

            ImageXRatio =  container.RenderSize.Width /(imageSource.PixelWidth);
            ImageYRatio =  container.RenderSize.Height /(imageSource.PixelHeight);

            OnPropertyChanged("ImageXRatio");
            OnPropertyChanged("ImageYRatio");
        }

        public double ImageYRatio { get; private set; }

        

        public ProjectViewModel(string imageFile, Image container)
        {
            this.container = container;
            ImageFile = imageFile;
            project = Project.Create(imageFile);
            var analayzer = new LuxlandRecognitionEngine();
            OriginalFeatures = analayzer.ExtractFacialFutures(new Bitmap(imageFile)).ToList();
            container.SizeChanged += (sender, args) =>
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