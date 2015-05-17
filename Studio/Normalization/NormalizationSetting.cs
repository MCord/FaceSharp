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
    }
}