//namespace Studio
//{
//    using System;
//    using System.Collections.Generic;
//    using System.Drawing;
//    using System.Windows.Media.Imaging;
//    using Luxand;

//    public class RotationNormalization : INormalization
//    {
//        public BitmapImage Apply(BitmapImage image, List<FacialFeature> features)
//        {
//            var noseTop = features.Find(f => f.Id == (int)FSDK.FacialFeatures.FSDKP_LEFT_EYE);
//            var rightEye = features.Find(f => f.Id == (int)FSDK.FacialFeatures.FSDKP_RIGHT_EYE);

//            var yDelta = (rightEye.Location.Y - noseTop.Location.Y);
//            var xDelta = (rightEye.Location.X - noseTop.Location.X);

//            var degree = ((Math.Atan2( yDelta, xDelta ) * 180.0/ Math.PI));

//            foreach (var feat in features)
//            {
//                feat.Location = RotatePoint(feat.Location, new Point(0,0), -degree);
//            }

//            return RotateImageByAngle(image.ToBitmap(), new Point(0,0), (float) -degree).ToBitmapImage();
//        }

//        static Point RotatePoint(Point pointToRotate, Point centerPoint, double angleInDegrees)
//        {
//            double angleInRadians = angleInDegrees * (Math.PI / 180);
//            double cosTheta = Math.Cos(angleInRadians);
//            double sinTheta = Math.Sin(angleInRadians);
//            return new Point
//            {
//                X =
//                    (int)
//                    (cosTheta * (pointToRotate.X - centerPoint.X) -
//                    sinTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.X),
//                Y =
//                    (int)
//                    (sinTheta * (pointToRotate.X - centerPoint.X) +
//                    cosTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.Y)
//            };
//        }

//        /// <summary>
//        /// Rotates the image by angle.
//        /// </summary>
//        /// <param name="oldBitmap">The old bitmap.</param>
//        /// <param name="angle">The angle.</param>
//        /// <returns></returns>
//        private static Bitmap RotateImageByAngle(Image oldBitmap, Point origin, float angle)
//        {
//            //var newBitmap = new Bitmap(oldBitmap.Width, oldBitmap.Height +(int) (0.2 * oldBitmap.Height));
//            //var graphics = Graphics.FromImage(newBitmap);

//            //float deltax = (oldBitmap.Width/2) - origin.X;
//            //float deltaY = (oldBitmap.Height/2) - origin.Y;

//            //graphics.TranslateTransform(deltax, deltaY);
//            //graphics.RotateTransform(angle);
//            //graphics.DrawImage(oldBitmap, new Point(0, 0));
//            //graphics.TranslateTransform(-deltax, -deltaY);
//            return oldBitmap;
//        }
//    }
//}