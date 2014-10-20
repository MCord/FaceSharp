namespace Studio
{
    using System;
    using System.Windows;
    using System.Windows.Media.Imaging;
    using Microsoft.Win32;

    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private Project currentProject;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ImportMenuClicked(object sender, RoutedEventArgs e)
        {
            var fd = new OpenFileDialog();
            
            if (fd.ShowDialog().GetValueOrDefault())
            {
                CreateNewProject(fd.FileName);
            }
        }

        private void CreateNewProject(string file)
        {
            currentProject = Project.Create(file);
            ManualBind();
        }

        private void ManualBind()
        {
            CurrentImage.Source = new BitmapImage(new Uri(currentProject.File));
        }
    }
}