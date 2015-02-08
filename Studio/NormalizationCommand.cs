namespace Studio
{
    using System;
    using System.Linq;
    using System.Windows.Input;

    public class NormalizationCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            var project = (Project) parameter ;
            return !project.Normalizations.Any();
        }

        public void Execute(object parameter)
        {
            var project = (Project)parameter;

            //project.ApplyNormalization(new RotationNormalization());
            //project.ApplyNormalization(new MoveNormalization());

            CanExecuteChanged?.Invoke(this, null);
        }

        public event EventHandler CanExecuteChanged;
    }
}