namespace Studio
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Normalization;

    class FaceoffNormalizer : ImageSetNormalizer
    {
        private readonly int[] s2;

        public FaceoffNormalizer(string refFile, string normFile, string targetFolder, int[] s2) : base(new WarperSetting(refFile, normFile, targetFolder))
        {
            this.s2 = s2;
        }

        protected override FacialFeature CalulateAverageTargetForPoint(List<List<FacialFeature>> featureSamples, FacialFeature f)
        {
            if (s2.Contains(f.Id))
            {
                return featureSamples[1].First(p => p.Id == f.Id);
            }

            return f;
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