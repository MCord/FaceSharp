using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Windows;
using System.Windows.Media.Imaging;
using WarpImage;
using Point = System.Drawing.Point;

namespace Studio.Common
{
    class WarpImageProcessor : IImageProcessor
    {
        private static readonly MovingLeastSquaresRectGrid _mls = new MovingLeastSquaresRectGrid();

        private readonly List<Point> targetPointSet;

        public WarpImageProcessor(List<Point> targetPointSet)
        {
            this.targetPointSet = targetPointSet;
        }

        public ProcessedImage Process(ProcessedImage source)
        {
            return source.Process(WarpImage);
        }

        private ProcessedImage WarpImage(Image arg, List<FacialFeature> features)
        {
            var sourcePoints = GetSourcePoints(features);

            _mls.InitBeforeComputation(Convert(sourcePoints), Convert(targetPointSet), arg.Height, arg.Width);

            var convertBitmap = ConvertBitmap((Bitmap)arg);
            var pixels = Image2PixelArray.GetPixelsTopLeft(convertBitmap);

            var bmp = _mls.WarpImage(pixels, convertBitmap.DpiX, convertBitmap.DpiY);

            return new ProcessedImage(BitmapFromWriteableBitmap(bmp), features);
        }

        private Bitmap BitmapFromWriteableBitmap(WriteableBitmap writeBmp)
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
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                source.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
        }

        private System.Windows.Point[] Convert(IEnumerable<Point> points)
        {
            return points.Select(p => new System.Windows.Point(p.X, p.Y)).ToArray();
        }

        private Point[] GetSourcePoints(List<FacialFeature> features)
        {
            return features.Select(f => f.Location).ToArray();
        }
    }
}