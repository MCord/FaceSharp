namespace Studio.Interface
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Forms;

    /// <summary>
    ///     Interaction logic for FaceOffWindow.xaml
    /// </summary>
    public partial class FaceOffWindow : Window
    {
        private readonly FaceOffViewModel model;

        public FaceOffWindow()
        {
            InitializeComponent();
            model = new FaceOffViewModel();
            DataContext = model;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            SetPropertyFromFilePath(file => model.FirstFile = file);
        }

        private void SetPropertyFromFilePath(Action<string> set)
        {
            var ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                set(ofd.FileName);
            }
        }

        private void button1_Copy_Click(object sender, RoutedEventArgs e)
        {
            SetPropertyFromFilePath(file => model.SecondFile = file);
        }

        private void button1_Copy1_Click(object sender, RoutedEventArgs e)
        {
            var odd = new FolderBrowserDialog();
            if (odd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var op = new FaceoffNormalizer(model.FirstFile, model.SecondFile, odd.SelectedPath, model.S2Set.Split(',').Select(s=>int.Parse(s.Trim())).ToArray());
                op.Normalize();
            }
        }
    }
}