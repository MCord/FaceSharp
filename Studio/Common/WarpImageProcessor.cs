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

            Execute(arg, sourcePoints);

            var convertBitmap = ConvertBitmap((Bitmap)arg);
            var pixels = Image2PixelArray.GetPixelsTopLeft(convertBitmap);

            var bmp = _mls.WarpImage(pixels, convertBitmap.DpiX, convertBitmap.DpiY);

            return new ProcessedImage(BitmapFromWriteableBitmap(bmp), features);
        }

        private void Execute(Image arg, PointF[] sourcePoints)
        {
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
            _mls.InitBeforeComputation(fromFilter.ToArray(), toFilter.ToArray(), arg.Height, arg.Width, stepSize);

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

        private PointF[] Convert(IEnumerable<PointF> points)
        {
            return points.Select(p => new PointF(p.X, p.Y)).ToArray();
        }

        private PointF[] GetSourcePoints(List<FacialFeature> features)
        {
            return features.Select(f => new PointF(f.Location.X, f.Location.Y)).ToArray();
        }
    }
}