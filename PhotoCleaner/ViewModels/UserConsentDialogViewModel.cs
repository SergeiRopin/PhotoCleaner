using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PhotoCleaner.ViewModels
{
    public class UserConsentDialogViewModel : INotifyPropertyChanged
    {
        public UserConsentDialogViewModel(string message)
        {
            Message = message;
        }

        public string Message { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
