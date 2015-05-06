using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Studio.Common;

namespace Studio
{
    public class ImageManipulator
    {
        private const int ImageSize = 500;
        private readonly LuxlandRecognitionEngine _engine;

        public ImageManipulator()
        {
            _engine = new LuxlandRecognitionEngine();
        }

        private Point FindFaceCenter(List<FacialFeature> features)
        {
            return features.First(f => f.Id == 22).Location;
        }

        public void NormalizeFolder(string path)
        {
            var images = Directory.GetFiles(path).Select(Image.FromFile).ToArray();

            foreach (var image in images)
            {
                ToBigCanvas(image);
            }
        }

        public void ToBigCanvas(params Image[] images)
        {
            var processor = new ImageProcessorChain
            {
                new NormalizedFaceSizeImageScaler(),
                new AutoFaceTiltProcessor(),
                new ImageOpacityProcessor(1f),
                new FaceCenteredCropProcessor(500)
            };

            foreach (var image in images)
            {
                var features = _engine.ExtractFacialFutures(image).ToList();

                if (!features.Any())
                {
                    return;
                }

                var pi = processor.Process(new ProcessedImage(image, features));

                pi.Save(string.Format("H:\\pi\\{0}.jpeg", Guid.NewGuid()));
            }
        }
    }
}