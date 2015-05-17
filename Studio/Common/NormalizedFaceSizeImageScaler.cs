namespace Studio.Common
{
    class NormalizedFaceSizeImageScaler : ImageScaler
    {
        private readonly double referenceEyeSize;

        public NormalizedFaceSizeImageScaler(double referenceEyeSize = 50.0) : base(1)
        {
            this.referenceEyeSize = referenceEyeSize;
        }

        public override double GetScaleRatio(ProcessedImage source)
        {
            return referenceEyeSize / Geometry.GetDistance(source[25], source[26]);
        }
    }
}