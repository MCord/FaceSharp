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
    using Graphics;

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

                CurrentImage = WarpImage(OriginalImage).ToBitmapImage();

                Storage.SaveData(StorageItem.LastLoadedImage, imageFile);
                return;
            }

            throw new FileNotFoundException("The selected file does not exist.");
        }

        private WriteableBitmap WarpImage(BitmapImage originalImage)
        {
            var rand = new Random();

            var mlsAlgo = new MovingLeastSquaresRectGrid();

            var originalPoints = Features.Select(f => f.Location).Select(l => new System.Windows.Point(l.X, l.Y)).ToArray();
            var transformPoint = originalPoints.Select(o => new System.Windows.Point(o.X + 100 * rand.NextDouble(), o.Y-5)).ToArray();

            var pixels = Image2PixelArray.GetPixelsTopLeft(originalImage);

            int h = Image2PixelArray.GetH(pixels);
            int w = Image2PixelArray.GetW(pixels);


            mlsAlgo.InitBeforeComputation(originalPoints, transformPoint, h, w, 20);

            return mlsAlgo.WarpImage(pixels, originalImage.DpiX, originalImage.DpiY);
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