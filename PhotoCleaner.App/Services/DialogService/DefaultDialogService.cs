using PhotoCleaner.App.Models;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;

namespace PhotoCleaner.App.Services.DialogService
{
    public class DefaultDialogService : IDialogService
    {
        public IEnumerable<SelectedFile> SelectedFiles { get; private set; }
        public string SelectedDirectory { get; set; }

        public bool OpenFileDialog(string fileExtension, string initialDirectory)
        {
            var openDialog = new OpenFileDialog
            {
                Filter = fileExtension,
                Multiselect = true,
                InitialDirectory = initialDirectory ?? string.Empty
            };

            if (openDialog.ShowDialog() == true)
            {
                SelectedFiles = openDialog.FileNames.Select(x =>
                new SelectedFile
                {
                    Path = x,
                    Name = System.IO.Path.GetFileName(x),
                    NameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(x),
                    Extension = System.IO.Path.GetExtension(x)
                });
                if (SelectedFiles.Any())
                    SelectedDirectory = System.IO.Path.GetDirectoryName(SelectedFiles.First().Path);
                return true;
            }
            return false;
        }
    }
}
