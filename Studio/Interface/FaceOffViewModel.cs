namespace Studio.Interface
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using Annotations;

    public class FaceOffViewModel : INotifyPropertyChanged
    {
        private string firstFile;
        private string secondFile;
        private string s2Set;

        public FaceOffViewModel()
        {
            s2Set = "0,1,24,23,38,27,37,35,28,36,29,30,25,26,41,31,42,40,32,39,33,34";
        }

        public string S2Set
        {
            get { return s2Set; }
            set
            {
                s2Set = value;
                OnPropertyChanged();
            }
        }

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