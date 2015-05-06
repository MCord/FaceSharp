using System;
using System.Drawing;

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
            return referenceEyeSize / GetDistance(source[25], source[26]);
        }

        private double GetDistance(Point left, Point right)
        {
            var dx = left.X - right.X;
            var dy = left.Y - right.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }
    }
}