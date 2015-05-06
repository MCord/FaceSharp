using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace Studio.Common
{
    public class ProcessedImage
    {
        private readonly List<FacialFeature> extractedFeatures;
        private readonly Image image;

        public ProcessedImage(Image image, List<FacialFeature> extractedFeatures)
        {
            this.image = image;
            this.extractedFeatures = extractedFeatures;
        }

        public ProcessedImage Process(Func<Image, Image> operation, Func<List<FacialFeature>, List<FacialFeature>> featureOperation = null)
        {
            featureOperation = featureOperation ?? (list => list);

            return new ProcessedImage(operation(image), featureOperation(extractedFeatures));
        }

        public Point this[int index]
        {
            get { return extractedFeatures.First(f => f.Id == index).Location; }
        }

        public void Save(string file)
        {
            image.Save(file,ImageFormat.Jpeg);
        }
    }
}