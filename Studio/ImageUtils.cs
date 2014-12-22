namespace Studio
{
    using System.Drawing;
    using System.IO;
    using System.Windows.Media.Imaging;

    internal static class ImageUtils
    {
        public static Bitmap ToBitmap(this BitmapImage bitmapImage)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);

                return new Bitmap(bitmap);
            }
        }

        public static BitmapSource ToImageSource(this Bitmap bmp)
        {
            using (var ms = new MemoryStream())
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();

                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                ms.Seek(0, SeekOrigin.Begin);
                bitmap.StreamSource = ms;
                bitmap.EndInit();
                return bitmap;
            }
        }
    }
}