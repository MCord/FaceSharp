using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;
using WarpImage;

namespace Studio.Common
{
    public class WarpImageProcessor : IImageProcessor
    {
        
        private readonly List<PointF> targetPointSet;
        private readonly int stepSize;

        public WarpImageProcessor(List<PointF> targetPointSet, int stepSize)
        {
            this.targetPointSet = targetPointSet;
            this.stepSize = stepSize;
        }

        public ProcessedImage Process(ProcessedImage source)
        {
            return source.Process(WarpImage);
        }

        private ProcessedImage WarpImage(Image arg, List<FacialFeature> features)
        {
            var sourcePoints = GetSourcePoints(features);

            var from = Convert(sourcePoints);
            var to = Convert(targetPointSet);


            var fromFilter = new List<PointF>();
            var toFilter = new List<PointF>();
            for (int i = 0; i < from.Length; i++)
            {
                if (i != 22)
                {
                    fromFilter.Add(from[i]);
                    toFilter.Add(to[i]);
                }
            }
            return Warp(arg, features, fromFilter, toFilter,stepSize);
        }

        public static ProcessedImage Warp(Image arg, List<FacialFeature> features, IEnumerable<PointF> fromFilter, IEnumerable<PointF> toFilter, int stepSize)
        {
            var mls = new MovingLeastSquaresRectGrid();
            mls.InitBeforeComputation(fromFilter.ToArray(), toFilter.ToArray(), arg.Height, arg.Width, stepSize);

            var convertBitmap = ConvertBitmap((Bitmap) arg);
            var pixels = Image2PixelArray.GetPixelsTopLeft(convertBitmap);

            var bmp = mls.WarpImage(pixels, convertBitmap.DpiX, convertBitmap.DpiY);
            return new ProcessedImage(BitmapFromWriteableBitmap(bmp), features);
        }

        private static Bitmap BitmapFromWriteableBitmap(WriteableBitmap writeBmp)
        {
            Bitmap bmp;
            MemoryStream outStream = new MemoryStream();
            BitmapEncoder enc = new BmpBitmapEncoder();
            enc.Frames.Add(BitmapFrame.Create(writeBmp));
            enc.Save(outStream);
            bmp = new Bitmap(outStream);
            return bmp;
        }

        private static BitmapSource ConvertBitmap(Bitmap source)
        {

            Rectangle rect = new Rectangle(0, 0, source.Width, source.Height);

            System.Drawing.Imaging.BitmapData bmpData = source.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppRgb);

            int bufferSize = bmpData.Stride * source.Height;

            WriteableBitmap wb = new WriteableBitmap(source.Width, source.Height, source.HorizontalResolution, source.VerticalResolution, PixelFormats.Bgr32, null);

            wb.WritePixels(new Int32Rect(0, 0, source.Width, source.Height), bmpData.Scan0, bufferSize, bmpData.Stride);

            // Unlock the bits.

            source.UnlockBits(bmpData);


            return wb;

        }

        private static PointF[] Convert(IEnumerable<PointF> points)
        {
            return points.Select(p => new PointF(p.X, p.Y)).ToArray();
        }

        private static PointF[] GetSourcePoints(List<FacialFeature> features)
        {
            return features.Select(f => new PointF(f.Location.X, f.Location.Y)).ToArray();
        }
    }

    public class TestCase
    {
        [XmlArray] public PointF[] First;
        [XmlArray] public PointF[] Second;
    }
}