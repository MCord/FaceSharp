using System;
using System.Collections.Generic;
using System.IO;

namespace Studio.Normalization
{
    public class NormalizationSetting
    {
        public string SourceFolder;
        public string TargetFolder;
        public int ImageWidth;
        public double ReferenceEyeSize;
        public int WarpStep;

        public NormalizationSetting()
        {
            WarpStep = 10;
            ImageWidth = 500;
            ReferenceEyeSize = 50.0;
        }

        public virtual IEnumerable<string> GetSourceFiles()
        {
            return Directory.GetFiles(SourceFolder);
        }

        public virtual string GetOutputFileName(string source)
        {
            return Path.Combine(TargetFolder, Path.GetFileName(source) ?? $"{Guid.NewGuid()}.jpeg");
        }
    }
}