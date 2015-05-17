using System.IO;

namespace Studio
{
    using System;
    using System.Windows;
    using Microsoft.Win32;

    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
                InitializeComponent();
            try
            {
                LoadLastProject();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(),"Startup error", MessageBoxButton.OK,MessageBoxImage.Error);
            }
         
        }

        private void LoadLastProject()
        {
            var lastProj = Storage.LoadData(StorageItem.LastLoadedImage);

            if (lastProj != null)
            {
                CreateNewProject(lastProj);
            }
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
            DataContext = new ProjectViewModel(file, ImageControl);
        }
    }
}