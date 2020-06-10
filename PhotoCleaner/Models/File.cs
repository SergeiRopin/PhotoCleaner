using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PhotoCleaner.Models
{
    public class @File : INotifyPropertyChanged
    {
        private string _name = string.Empty;
        private bool _isUnique;
        private bool _isEmpty;

        public string Path { get; set; } = string.Empty;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }
        public string NameWithoutExtension { get; set; } = string.Empty;
        public string Extension { get; set; }
        public bool IsUnique
        {
            get => _isUnique;
            set
            {
                _isUnique = value;
                OnPropertyChanged("IsUnique");
            }
        }
        public bool IsEmpty
        {
            get => _isEmpty;
            set
            {
                _isEmpty = value;
                OnPropertyChanged("IsEmpty");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
