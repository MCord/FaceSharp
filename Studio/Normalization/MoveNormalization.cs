namespace Studio
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Linq;
    using System.Windows.Media.Imaging;

    public class MoveNormalization : INormalization
    {
        public BitmapImage Apply(BitmapImage image, List<FacialFeature> features)
        {
            var center = features.First(f => f.Id == 22).Location;


            var newImg = new Bitmap(300, 300,PixelFormat.Format32bppArgb);

            var g = Graphics.FromImage(newImg);
            
            g.TranslateTransform((newImg.Width/2) - center.X, (newImg.Height/2) - center.Y);
            g.DrawImageUnscaled(image.ToBitmap(), 0, 0); 

            //g.DrawEllipse(Pens.Orange, center.X-5, center.Y-5, 10, 10);
            g.Dispose();
            return newImg.ToBitmapImage();
        }
    }
}