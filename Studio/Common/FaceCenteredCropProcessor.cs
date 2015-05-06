using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Studio.Common
{
    public class FaceCenteredCropProcessor : IImageProcessor
    {
        private readonly int imageSize;

        public FaceCenteredCropProcessor(int imageSize)
        {
            this.imageSize = imageSize;
        }

        public ProcessedImage Process(ProcessedImage source)
        {
            return source.Process(i => DrawImage(i, source[22]));
        }

        private Image DrawImage(Image original, Point faceCenter)
        {
            var result = new Bitmap(imageSize, imageSize, PixelFormat.Format32bppArgb);
            var g = System.Drawing.Graphics.FromImage(result);
            g.FillRectangle(new HatchBrush(HatchStyle.LargeCheckerBoard, Color.WhiteSmoke), 0, 0, result.Width, result.Height);

            var resultCenter = new Point(imageSize / 2, imageSize / 2);

            var delta = new Point(resultCenter.X - faceCenter.X, resultCenter.Y - faceCenter.Y);

            g.DrawImageUnscaledAndClipped(original, new Rectangle(delta, original.Size));

            return result;
        }

    }
}