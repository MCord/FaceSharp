using System.Collections.Generic;
using System.IO;
using System.Linq;
using Studio.Normalization;

namespace Studio
{
    class FaceWarperNormalizer : ImageSetNormalizer
    {
        public FaceWarperNormalizer(string refFile,string normFile, string targetFolder) : base(new WarperSetting(refFile, normFile, targetFolder))
        {
        }

        private readonly List<FacialFeature> pointsStorage = new List<FacialFeature>();

        private readonly int[] pointsToLock = {0,1,24,23,38,27,37,35,28,36,29,30,25,26,41,31,42,40,32,39,33,34};

        protected override FacialFeature CalulateAverageTargetForPoint(List<List<FacialFeature>> featureSamples, FacialFeature f)
        {
            if (pointsToLock.Contains(f.Id))
            {
                var cache = pointsStorage.FirstOrDefault(p => p.Id == f.Id);
                if (cache != null)
                {
                    return cache;
                }

                pointsStorage.Add(f);
                return f;
            }

            return base.CalulateAverageTargetForPoint(featureSamples, f);
        }

        private class WarperSetting : NormalizationSetting
        {
            private readonly string refFile;
            private readonly string normFile;
            private readonly string targetFolder;

            public WarperSetting(string refFile, string normFile, string targetFolder)
            {
                this.refFile = refFile;
                this.normFile = normFile;
                this.targetFolder = targetFolder;
            }

            public override IEnumerable<string> GetSourceFiles()
            {
                yield return refFile;
                yield return normFile;
            }

            public override string GetOutputFileName(string source)
            {
                return Path.Combine(targetFolder, Path.GetFileName(source));
            }
        }
    }

    
}