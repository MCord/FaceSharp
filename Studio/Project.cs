namespace Studio
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
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
        public List<FacialFeature> OriginalFeatures { get; private set; }
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

        public BitmapImage CurrentImage
        {
            get { return currentImage; }
            set
            {
                currentImage = value;
                OnPropertyChanged();
            }
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
            OriginalFeatures = analayzer.ExtractFacialFutures(image).ToList();
            OnPropertyChanged("OriginalFeatures");
        }

        public static Project Create(string file)
        {
            var project = new Project("", "");
            project.Import(file);
            return project;
        }

        public void ApplyNormalization(RotationNormalization norm)
        {
            CurrentImage = norm.Apply(CurrentImage, OriginalFeatures);
            ReCalculateFeatures(CurrentImage.ToBitmap());
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}