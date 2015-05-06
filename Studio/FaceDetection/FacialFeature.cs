using System;

namespace Studio
{
    using System.Drawing;

    public class FacialFeature
    {
        public int Id { get; }
        public string Name { get; }
        public Point Location { get; }

        public FacialFeature(int id, string name, Point location)
        {
            Id = id;
            Name = name;
            Location = location;
        }

        public FacialFeature Scale(double ratio)
        {
            return new FacialFeature(Id, Name, new Point((int) (Location.X * ratio), (int) (Location.Y * ratio)));
        }

        public FacialFeature Rotate(PointF offset, float angle)
        {
            var radian = angle*Math.PI/180;

            var rotatedPoint = new Point
            {
                X = (int) (Math.Cos(radian)*(Location.X - offset.X) - Math.Sin(radian)*(Location.Y - offset.Y) + offset.X),
                Y = (int) (Math.Sin(radian)*(Location.X - offset.X) + Math.Cos(radian)*(Location.Y - offset.Y) + offset.Y),
            };


            return new FacialFeature(Id, Name,rotatedPoint);
        }
    }
}