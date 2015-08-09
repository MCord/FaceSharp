using System;
using System.Windows.Input;
using Studio.Interface;

namespace Studio
{
    public class FaceWarpToolCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var w = new FaceWarpWindow();
            w.ShowDialog();
        }
    }
}