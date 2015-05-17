using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;

namespace Studio
{
    public class NormalizationToolCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var ofd = new OpenFileDialog();

            try
            {
                if (ofd.ShowDialog().GetValueOrDefault())
                {
                    var log = ImageManipulator.NormalizeFromSettingFile(ofd.FileName);
                    var temp = Path.GetTempFileName();
                    File.WriteAllLines(temp, log);

                    Process.Start("Notepad.exe", temp);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event EventHandler CanExecuteChanged;
    }
}