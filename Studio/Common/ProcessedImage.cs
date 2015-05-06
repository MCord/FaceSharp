using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace Studio.Common
{
    public class ProcessedImage
    {
        public readonly List<FacialFeature> ExtractedFeatures;
        public readonly Image Image;

        public ProcessedImage(Image image, List<FacialFeature> extractedFeatures)
        {
            this.Image = image;
            this.ExtractedFeatures = extractedFeatures;
        }

        public ProcessedImage Process(Func<Image, Image> operation, Func<List<FacialFeature>, List<FacialFeature>> featureOperation = null)
        {
            featureOperation = featureOperation ?? (list => list);
            return new ProcessedImage(operation(Image), featureOperation(ExtractedFeatures));
        }

        public ProcessedImage Process(Func<Image, List<FacialFeature>, ProcessedImage> operation)
        {
            return operation(Image, ExtractedFeatures);
        }

        public Point this[int index]
        {
            get { return ExtractedFeatures.First(f => f.Id == index).Location; }
        }

        public void Save(string file)
        {
            Image.Save(file,ImageFormat.Jpeg);
        }
    }
}