using System;
using System.Windows;
using System.Windows.Forms;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace Studio.Interface
{
    /// <summary>
    ///     Interaction logic for FaceWarpWindow.xaml
    /// </summary>
    public partial class FaceWarpWindow : Window
    {
        private readonly FaceWarpViewModel model;

        public FaceWarpWindow()
        {
            InitializeComponent();
            model = new FaceWarpViewModel();
            DataContext = model;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            SetPropertyFromFilePath(file => model.FirstFile = file);
        }

        private void SetPropertyFromFilePath(Action<string> set)
        {
            var ofd = new OpenFileDialog();
            if (ofd.ShowDialog().GetValueOrDefault())
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
                var op = new FaceWarperNormalizer(model.FirstFile, model.SecondFile, odd.SelectedPath);
                op.Normalize();
            }
        }
    }
}