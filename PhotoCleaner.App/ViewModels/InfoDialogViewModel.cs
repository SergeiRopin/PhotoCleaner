using PhotoCleaner.App.Domain;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PhotoCleaner.App.ViewModels
{
    public class InfoDialogViewModel : INotifyPropertyChanged
    {
        public InfoDialogViewModel(string message, ClearOperationResultType operationResult)
        {
            Message = message;
            Type = GetDialogType(operationResult);
        }

        public string Message { get; private set; }
        public InfoDialogType Type { get; private set; }

        private InfoDialogType GetDialogType(ClearOperationResultType operationResult)
        {
            if (operationResult == ClearOperationResultType.Success)
                return InfoDialogType.Success;
            if (operationResult == ClearOperationResultType.Fail)
                return InfoDialogType.Error;
            return InfoDialogType.NotFound;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
