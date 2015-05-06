using System;
using System.Collections.Generic;
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

            ToBigCanvas(images);
        }

        public void ToBigCanvas(params Image[] images)
        {
            var normalizedFiles = NormalizeImages(images);

            var facialNorm = FindFacialNorm(normalizedFiles).ToList();

            var warper = new WarpImageProcessor(facialNorm);


            foreach (var nf in normalizedFiles)
            {
                var image = Image.FromFile(nf);
                var facialFeatures = engine.ExtractFacialFutures(image).ToList();
                var pi = new ProcessedImage(image, facialFeatures);
                var warpedFileName = string.Format("{0}.warped.jpeg", nf);
                warper.Process(pi).Save(warpedFileName);
            }

        }
        private IEnumerable<Point> FindFacialNorm(List<string> normalizedFiles)
        {
            var featureSamples = new List<List<FacialFeature>>();
            foreach (var nf in normalizedFiles)
            {
                var image = Image.FromFile(nf);
                var facialFeatures = engine.ExtractFacialFutures(image).ToList();
                featureSamples.Add(facialFeatures);
            }

            return GetAverage(featureSamples).Select(f=>f.Location);
        }

        private List<string> NormalizeImages(Image[] images)
        {
            var processor = new ImageProcessorChain
            {
                new NormalizedFaceSizeImageScaler(),
                new AutoFaceTiltProcessor(),
                new ImageOpacityProcessor(1f),
                new FaceCenteredCropProcessor(500)
            };


            var normalizedFiles = new List<string>();

            var warpedImages = new List<Image>();
            foreach (var image in images)
            {
                var features = engine.ExtractFacialFutures(image).ToList();

                if (!features.Any())
                {
                    continue;
                }

                var pi = processor.Process(new ProcessedImage(image, features));

                var fileName = string.Format("H:\\pi\\{0}.jpeg", Guid.NewGuid());
                normalizedFiles.Add(fileName);
                warpedImages.Add(pi.Image);
                pi.Save(fileName);
            }

            var merged  = ImageOpacityProcessor.MergeWithOpacity(50, warpedImages.ToArray());
            merged.Save("H:\\merge.jpeg");

            return normalizedFiles;
        }

        private IEnumerable<FacialFeature> GetAverage(List<List<FacialFeature>> featureSamples)
        {
            var first = featureSamples.First();

            foreach (var f in first)
            {
                yield return new FacialFeature(f.Id, f.Name, AveragePoint(f.Id, featureSamples));
            }
        }

        private Point AveragePoint(int id, List<List<FacialFeature>> featureSamples)
        {
            var filter = featureSamples.Select(fs => fs.First(i => i.Id == id).Location).ToList();
            return new Point((int) filter.Average(f => f.X), (int) filter.Average(f => f.Y));
        }
    }
}