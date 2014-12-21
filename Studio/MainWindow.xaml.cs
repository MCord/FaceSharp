namespace Studio
{
    using System.Windows;
    using Microsoft.Win32;

    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private ProjectViewModel viewModel;

        public MainWindow()
        {
            InitializeComponent();
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
            viewModel = new ProjectViewModel(file);
            ManualBind();
        }

        private void ManualBind()
        {
            CurrentImage.Source = viewModel.CurrentImage;
        }
    }
}