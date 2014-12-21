namespace Studio
{
    using System.Drawing;

    public class FacialFeature
    {
        int Id { get; }
        string Name { get; }
        Point Location { get; }

        public FacialFeature(int id, string name, Point location)
        {
            Id = id;
            Name = name;
            Location = location;
        }
    }
}