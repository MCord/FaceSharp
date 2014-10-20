namespace Studio
{
    using System;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    public class ProjectViewModel
    {
        private readonly Project project;

        public ProjectViewModel(string file)
        {
            project = Project.Create(file);
        }

        public ImageSource CurrentImage
        {
            get { return new BitmapImage(new Uri(project.File)); }
        }
    }
}