using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Studio.Common
{
    class AutoFaceTiltProcessor : ImageRotator
    {
        protected override List<FacialFeature> GetRotatedFeatures(List<FacialFeature> facialFeatures, ProcessedImage source)
        {
            var offset = GetOffset(source);
            var angle = GetAngle(source);
            return facialFeatures.Select(f => f.Rotate(offset, angle)).ToList();
        }

        protected override float GetAngle(ProcessedImage source)
        {
            return (float) (90 - Angulo(source[22], source[49]));
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

        protected override PointF GetOffset(ProcessedImage source)
        {
            return source[22];
        }
    }
}