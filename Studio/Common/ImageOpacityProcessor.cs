using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows;

namespace Studio.Common
{
    class ImageOpacityProcessor : IImageProcessor
    {
        private readonly float opacity;

        public ImageOpacityProcessor(float opacity)
        {
            this.opacity = opacity;
        }

        public ProcessedImage Process(ProcessedImage source)
        {
            return source.Process(i => MergeWithOpacity(opacity, i));
        }

        public static Image MergeWithOpacity(float opacity, params Image[] images)
        {
            try
            {
                //create a Bitmap the size of the image provided  
                Bitmap bmp = new Bitmap(images[0].Width, images[0].Height);

                //create a graphics object from the image  
                using (System.Drawing.Graphics gfx = System.Drawing.Graphics.FromImage(bmp))
                {

                    //create a color matrix object  
                    ColorMatrix matrix = new ColorMatrix();

                    //set the opacity  
                    matrix.Matrix33 = opacity;

                    //create image attributes  
                    ImageAttributes attributes = new ImageAttributes();

                    //set the color(opacity) of the image  
                    attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                    //now draw the image  
                    foreach (var image in images)
                    {
                        gfx.DrawImage(image, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);
                    }
                }
                return bmp;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
    }
}