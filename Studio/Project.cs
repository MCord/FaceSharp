namespace Studio
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;
    using System.Windows.Media.Imaging;
    using Annotations;

    [DataContract]
    public class Project : INotifyPropertyChanged
    {
        private BitmapImage currentImage;
        public List<INormalization> Normalizations;
        public List<FacialFeature> Features { get; private set; }
        readonly LuxlandRecognitionEngine analayzer;
        private Project(string file, string name)
        {
            Normalizations = new List<INormalization>();
            File = file;
            analayzer = new LuxlandRecognitionEngine();
            Name = name;
        }

        public string File { get; private set; }
        public BitmapImage OriginalImage { get; private set; }

        public BitmapImage OverlayedImage => AddOverlay(currentImage);

        public BitmapImage CurrentImage
        {
            get
            {
                return currentImage;
            }
            set
            {
                currentImage = value;
                OnPropertyChanged();
            }
        }

        private BitmapImage AddOverlay(BitmapImage bitmapImage)
        {
            var image = new Bitmap(bitmapImage.PixelWidth, bitmapImage.PixelHeight,PixelFormat.Format32bppArgb);
            var g = System.Drawing.Graphics.FromImage(image);

            g.DrawImage(bitmapImage.ToBitmap(), 0,0);
            foreach (var f in Features)
            {
                g.DrawEllipse(Pens.Magenta, f.Location.X, f.Location.Y, 2, 3);
            }
            
            return image.ToBitmapImage();
        }

        public string Name { get; private set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public void Save()
        {
            this.SerializeToFile(File);
        }

        public static Project Load(string file)
        {
            var instance = SerializationExtensions.Deserialize<Project>(file);
            instance.File = file;
            return instance;
        }

        public void Import(string imageFile)
        {
            if (System.IO.File.Exists(imageFile))
            {
                File = imageFile;
                OriginalImage = currentImage = new BitmapImage(new Uri(imageFile));
                
                ReCalculateFeatures(OriginalImage.ToBitmap());

                Storage.SaveData(StorageItem.LastLoadedImage, imageFile);
                return;
            }

            throw new FileNotFoundException("The selected file does not exist.");
        }

       


        private void ReCalculateFeatures(Bitmap image)
        {
            Features = analayzer.ExtractFacialFutures(image).ToList();
            OnPropertyChanged("Features");
        }

        public static Project Create(string file)
        {
            var project = new Project("", "");
            project.Import(file);
            return project;
        }

        public void ApplyNormalization(INormalization norm)
        {
            CurrentImage = norm.Apply(CurrentImage, Features);
            OnPropertyChanged("Features");
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (propertyName == "CurrentImage")
            {
                OnPropertyChanged("OverlayedImage");
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}