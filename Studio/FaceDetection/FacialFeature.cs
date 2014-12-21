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
    }
}