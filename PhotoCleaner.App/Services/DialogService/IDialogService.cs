using PhotoCleaner.App.Models;
using System.Collections.Generic;

namespace PhotoCleaner.App.Services.DialogService
{
    public interface IDialogService
    {
        IEnumerable<File> SelectedFiles { get; }
        string SelectedDirectory { get; }
        bool OpenFileDialog(string extension, string directory);
    }
}
