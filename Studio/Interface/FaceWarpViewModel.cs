using System.ComponentModel;
using System.Runtime.CompilerServices;
using Studio.Annotations;

namespace Studio.Interface
{
    public class FaceWarpViewModel : INotifyPropertyChanged
    {
        private string firstFile;
        private string secondFile;

        public string FirstFile
        {
            get { return firstFile; }
            set
            {
                if (value == firstFile) return;
                firstFile = value;
                OnPropertyChanged();
            }
        }

        public string SecondFile
        {
            get { return secondFile; }
            set
            {
                if (value == secondFile) return;
                secondFile = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}