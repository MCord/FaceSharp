using System;
using System.Drawing;
using System.IO;
using System.Linq;
using Studio.Common;

namespace Studio
{
    public class ImageManipulator
    {
        private readonly LuxlandRecognitionEngine engine;

        public ImageManipulator()
        {
            engine = new LuxlandRecognitionEngine();
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
                var features = engine.ExtractFacialFutures(image).ToList();

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