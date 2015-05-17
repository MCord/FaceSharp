using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
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
            return source.Process(i=>Resize(i, ratio),f=> ScaleFutures(f,ratio));
        }

        public static Image Resize(Image source, double rate)
        {
            Bitmap newImage = new Bitmap((int) (source.Width * rate), (int)(source.Height* rate));
            using (Graphics gr = Graphics.FromImage(newImage))
            {
                gr.SmoothingMode = SmoothingMode.HighQuality;
                gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
                gr.PixelOffsetMode = PixelOffsetMode.HighQuality;
                gr.DrawImage(source, new Rectangle(0, 0, newImage.Width, newImage.Height));
            }

            return newImage;
        }
        private List<FacialFeature> ScaleFutures(List<FacialFeature> arg, double ratio)
        {
            return arg.Select(a => a.Scale(ratio)).ToList();
        }
    }
}