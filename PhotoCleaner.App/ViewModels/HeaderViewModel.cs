using PhotoCleaner.App.Domain;
using PhotoCleaner.App.Localization;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PhotoCleaner.App.ViewModels
{
    public class HeaderViewModel : INotifyPropertyChanged
    {
        private FileType _filesType;

        public HeaderViewModel(FileType filesType)
        {
            _filesType = filesType;
        }

        public string HeaderMessage => "efvw4erfd";

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
