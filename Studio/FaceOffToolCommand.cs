namespace Studio
{
    using System;
    using System.Windows.Input;
    using Interface;

    public class FaceOffToolCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var w = new FaceOffWindow();
            w.ShowDialog();
        }
    }
}