using System;
using System.Drawing;

namespace Studio.Common
{
    public class Geometry
    {
        public static double GetDistance(Point left, Point right)
        {
            var dx = left.X - right.X;
            var dy = left.Y - right.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        public static double GetDistance(System.Windows.Point left, System.Windows.Point right)
        {
            var dx = left.X - right.X;
            var dy = left.Y - right.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }
    }
}