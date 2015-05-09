using System.Drawing;

namespace Studio
{
    public class SourcedImage
    {
        public SourcedImage(string file)
        {
            Image = Image.FromFile(file);
            Path = file;
        }

        public readonly Image Image;
        public readonly string Path;

        public static SourcedImage FromFile(string arg)
        {
            return new SourcedImage(arg);
        }
    }
}