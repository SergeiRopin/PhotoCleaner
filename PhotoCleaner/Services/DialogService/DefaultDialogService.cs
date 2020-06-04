using PhotoCleaner.Models;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;

namespace PhotoCleaner.Services.DialogService
{
    public class DefaultDialogService : IDialogService
    {
        public IEnumerable<@File> SelectedFiles { get; private set; }
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
                new @File
                {
                    Path = x,
                    Name = System.IO.Path.GetFileName(x),
                    NameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(x)
                });
                if (SelectedFiles.Any())
                    SelectedDirectory = System.IO.Path.GetDirectoryName(SelectedFiles.First().Path);
                return true;
            }
            return false;
        }
    }
}
