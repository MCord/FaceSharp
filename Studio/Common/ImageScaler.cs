using System;
using System.Collections.Generic;
using System.Linq;

namespace Studio.Common
{
    public class ImageScaler : IImageProcessor
    {
        private readonly double defaltScaleValue;

        public ImageScaler(double defaltScaleValue)
        {
            this.defaltScaleValue = defaltScaleValue;
        }

        public virtual double GetScaleRatio(ProcessedImage source)
        {
            return defaltScaleValue;
        }
        public ProcessedImage Process(ProcessedImage source)
        {
            var ratio = GetScaleRatio(source);

            return source.Process(image => image.GetThumbnailImage((int) (image.Width*ratio),
                (int) (image.Height*ratio), null, IntPtr.Zero),f=> ScaleFutures(f,ratio));
        }

        private List<FacialFeature> ScaleFutures(List<FacialFeature> arg, double ratio)
        {
            return arg.Select(a => a.Scale(ratio)).ToList();
        }
    }
}