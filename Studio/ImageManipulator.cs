using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows;
using Point = System.Drawing.Point;

namespace Studio
{
    public class ImageManipulator
    {
        private const int ImageSize = 500;

        readonly LuxlandRecognitionEngine _engine;

        public ImageManipulator()
        {
            _engine = new LuxlandRecognitionEngine();
        }

        private Point? FindFaceCenter(List<FacialFeature> features)
        {
            return features.FirstOrDefault(f => f.Id == 22)?.Location;
        }

        public void NormalizeFolder(string path)
        {
            var images = Directory.GetFiles(path).Select(Image.FromFile).ToArray();

            foreach (var image in images)
            {
                var ot = Path.Combine("H:\\NOR", Guid.NewGuid()+".jpeg");
                ToBigCanvas(image).Save(ot,ImageFormat.Jpeg);
            }
        }

        public Image ToBigCanvas(params Image[] images)
        {
            var result = new Bitmap(ImageSize, ImageSize, PixelFormat.Format32bppArgb);
            var g = System.Drawing.Graphics.FromImage(result);
            g.FillRectangle(new HatchBrush(HatchStyle.LargeCheckerBoard, Color.WhiteSmoke), 0, 0,result.Width, result.Height);

            var opacity = 1.0f;
            foreach (var image in images)
            {
                var features = _engine.ExtractFacialFutures(image).ToList();

                if (!features.Any())
                {
                    return image;
                }
                var center = FindFaceCenter(features);
                var sizeCorrectionRatio = FindEyeSizeCorrectionRatio(features);
                var tiltCorrection = FindFaceTiltCorrectionValue(features);
                if (center.HasValue)
                {
                    var cv = center.Value;

                    var imageOpacity = SetImageOpacity(image,opacity);
                    var resized = imageOpacity.GetThumbnailImage((int) (imageOpacity.Width*sizeCorrectionRatio),
                        (int) (imageOpacity.Height*sizeCorrectionRatio), null, IntPtr.Zero);

                    center = new Point((int)(cv.X * sizeCorrectionRatio), (int)(cv.Y * sizeCorrectionRatio));

                    var tilted = RotateImage(resized, (PointF) center, (float) tiltCorrection);

                    

                    DrawImage(tilted, g, center.Value);
                    opacity = Math.Max(0.2f, opacity - 0.5f);
                }
            }
            return result;
        }

        private double FindFaceTiltCorrectionValue(List<FacialFeature> features)
        {
            var centerPoint = GetfeatureLocationById(features, 22);
            var lower = GetfeatureLocationById(features, 49);

            return 90- Angulo(centerPoint, lower);
        }

        private Double FindEyeSizeCorrectionRatio(List<FacialFeature> features)
        {
            var left = GetfeatureLocationById(features,25);
            var right = GetfeatureLocationById(features,26);

            return 50.0/ GetDistance(left, right);
        }

        private double GetDistance(Point left, Point right)
        {
            var dx = left.X-right.X;
            var dy = left.Y-right.Y;
            return Math.Sqrt(dx*dx + dy*dy);
        }

        private static Point GetfeatureLocationById(List<FacialFeature> features, int i)
        {
            return features.First(f => f.Id == i).Location;
        }

        private static void DrawImage(Image original, System.Drawing.Graphics g, Point faceCenter)
        {
            var resultCenter = new Point(ImageSize/2, ImageSize/2);

            var delta = new Point(resultCenter.X - faceCenter.X, resultCenter.Y - faceCenter.Y);

            g.DrawImageUnscaledAndClipped(original, new Rectangle(delta, original.Size));
        }

        /// <summary>  
        /// method for changing the opacity of an image  
        /// </summary>  
        /// <param name="image">image to set opacity on</param>  
        /// <param name="opacity">percentage of opacity</param>  
        /// <returns></returns>  
        public static Image SetImageOpacity(Image image, float opacity)
        {
            try
            {
                //create a Bitmap the size of the image provided  
                Bitmap bmp = new Bitmap(image.Width, image.Height);

                //create a graphics object from the image  
                using (System.Drawing.Graphics gfx = System.Drawing.Graphics.FromImage(bmp))
                {

                    //create a color matrix object  
                    ColorMatrix matrix = new ColorMatrix();

                    //set the opacity  
                    matrix.Matrix33 = (float) opacity;

                    //create image attributes  
                    ImageAttributes attributes = new ImageAttributes();

                    //set the color(opacity) of the image  
                    attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                    //now draw the image  
                    gfx.DrawImage(image, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);
                }
                return bmp;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
        private double Angulo(Point p1, Point p2)
        {
            double degrees;

            // Avoid divide by zero run values.
            if (p2.X - p1.X == 0)
            {
                if (p2.Y > p1.Y)
                    degrees = 90;
                else
                    degrees = 270;
            }
            else
            {
                // Calculate angle from offset.
                double riseoverrun = (p2.Y - p1.Y) / (double)(p2.X - p1.X);
                double radians = Math.Atan(riseoverrun);
                degrees = radians * (180 / Math.PI);

                // Handle quadrant specific transformations.       
                if ((p2.X - p1.X) < 0 || (p2.Y - p1.Y) < 0)
                    degrees += 180;
                if ((p2.X - p1.X) > 0 && (p2.Y - p1.Y) < 0)
                    degrees -= 180;
                if (degrees < 0)
                    degrees += 360;
            }
            return degrees;
        }

        /// <summary>
        /// Creates a new Image containing the same image only rotated
        /// </summary>
        /// <param name=""image"">The <see cref=""System.Drawing.Image"/"> to rotate
        /// <param name=""offset"">The position to rotate from.
        /// <param name=""angle"">The amount to rotate the image, clockwise, in degrees
        /// <returns>A new <see cref=""System.Drawing.Bitmap"/"> of the same size rotated.</see>
        /// <exception cref=""System.ArgumentNullException"">Thrown if <see cref=""image"/"> 
        /// is null.</see>
        public static Bitmap RotateImage(Image image, PointF offset, float angle)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            //create a new empty bitmap to hold rotated image
            Bitmap rotatedBmp = new Bitmap(image.Width, image.Height);
            rotatedBmp.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            //make a graphics object from the empty bitmap
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(rotatedBmp);

            //Put the rotation point in the center of the image
            g.TranslateTransform(offset.X, offset.Y);

            //rotate the image
            g.RotateTransform(angle);

            //move the image back
            g.TranslateTransform(-offset.X, -offset.Y);

            //draw passed in image onto graphics object
            g.DrawImage(image, new PointF(0, 0));

            return rotatedBmp;
        }

    }
}