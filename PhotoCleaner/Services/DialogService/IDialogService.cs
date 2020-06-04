using PhotoCleaner.Models;
using System.Collections.Generic;

namespace PhotoCleaner.Services.DialogService
{
    public interface IDialogService
    {
        IEnumerable<File> SelectedFiles { get; }
        string SelectedDirectory { get; }
        bool OpenFileDialog(string extension, string directory);
    }
}
