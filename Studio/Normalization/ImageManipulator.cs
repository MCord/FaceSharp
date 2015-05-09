using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Studio.Common;
using Studio.Normalization;

namespace Studio
{
    public class ImageManipulator
    {
        private readonly NormalizationSetting settings;
        private readonly LuxlandRecognitionEngine engine;
        private static List<string> log = new List<string>();

        private void Log(string format, params object[] args)
        {
            log.Add(string.Format(format, args));
        }
        public ImageManipulator(NormalizationSetting settings)
        {
            this.settings = settings;
            engine = new LuxlandRecognitionEngine();
        }

        public List<string> Normalize()
        {
            try
            {
                log.Clear();
                var images = Directory.GetFiles(settings.SourceFolder).Select(SourcedImage.FromFile).ToArray();
                Log("Found {0} source files.", images.Count());
                NormalizeInternal(images);
                return log;
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
                throw new Exception(string.Join("\n", log));
            }
        }

        private void NormalizeInternal(params SourcedImage[] images)
        {
            var normalizedFiles = NormalizeAndSave(images);
            CreatedWarpedToNormFaces(normalizedFiles);
        }

        private void CreatedWarpedToNormFaces(List<string> normalizedFiles)
        {
            Log("Creating Warped faces from facial norm of the collection.");

            var facialNorm = FindFacialNorm(normalizedFiles).ToList();

            var warper = new WarpImageProcessor(facialNorm, settings.StepSize);


            foreach (var nf in normalizedFiles)
            {
                Log("Warping {0}", nf);
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

            return GetAverage(featureSamples).Select(f => f.Location);
        }

        private List<string> NormalizeAndSave(SourcedImage[] images)
        {
            Log("Normalizing files...");
            var processor = new ImageProcessorChain
            {
                new NormalizedFaceSizeImageScaler(settings.ReferenceEyeSize),
                new AutoFaceTiltProcessor(),
                new FaceCenteredCropProcessor(settings.ImageWidth)
            };


            var normalizedFiles = new List<string>();

            foreach (var image in images)
            {
                var features = engine.ExtractFacialFutures(image.Image).ToList();

                if (!features.Any())
                {
                    Log("No face detected from {0}. skipping", image.Path);
                    continue;
                }

                var pi = processor.Process(new ProcessedImage(image.Image, features));

                var fileName = Path.Combine(settings.TargetFolder, Path.GetFileName(image.Path) ?? Guid.NewGuid() + ".jpeg");
                Log("Normalized {0}", image.Path);
                normalizedFiles.Add(fileName);
                pi.Save(fileName);
            }

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
            return new Point((int)filter.Average(f => f.X), (int)filter.Average(f => f.Y));
        }

        public static List<string> NormalizeFromSettingFile(string file)
        {
            var im = new ImageManipulator(SerializationExtensions.Deserialize<NormalizationSetting>(file));
            return im.Normalize();
        }
    }
}