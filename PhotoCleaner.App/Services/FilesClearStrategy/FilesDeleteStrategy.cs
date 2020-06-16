using PhotoCleaner.App.Domain;
using PhotoCleaner.App.Localization;
using PhotoCleaner.App.Models;
using PhotoCleaner.App.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PhotoCleaner.App.Services.FilesClearStrategy
{
    public class FilesDeleteStrategy : IFilesClearStrategy
    {
        INotifyPropertyChanged _viewModel;
        bool _hasErrors = false;
        bool _filesNotFound = true;

        public FilesDeleteStrategy(INotifyPropertyChanged viewModel)
        {
            _viewModel = viewModel;
        }

        public async Task<ClearOperationResult> ClearFiles()
        {
            var mainViewModel = _viewModel as MainWindowViewModel;
            if (mainViewModel == null)
                throw new ArgumentException("Only MainWindowViewModel is accepted argument");

            var sourceFiles = mainViewModel.SourceFiles
                .Where(x => !string.IsNullOrEmpty(x.NameWithoutExtension))
                .Select(x => x.NameWithoutExtension);

            var targetFiles = mainViewModel.TargetFiles
                .Where(x => !string.IsNullOrEmpty(x.NameWithoutExtension))
                .Select(x => x.NameWithoutExtension);

            var namesToRemove = targetFiles.Except(sourceFiles).ToArray();

            var filesToRemove = new List<string>();
            var targetFileExtension = mainViewModel.TargetFiles.First(x => !string.IsNullOrEmpty(x.NameWithoutExtension)).Extension;
            filesToRemove.AddRange(
                namesToRemove.Select(name => 
                Path.Combine(mainViewModel.SelectedTargetDirectory, name) + targetFileExtension));

            await RemoveFiles(filesToRemove, mainViewModel.TargetFiles).ConfigureAwait(true);

            if (_hasErrors)
            {
                return new ClearOperationResult
                {
                    Type = ClearOperationResultType.Fail,
                    Message = DialogWindowStrings.DeleteFail
                };
            }
            if(!_hasErrors && _filesNotFound)
            {
                return new ClearOperationResult
                {
                    Type = ClearOperationResultType.NotFound,
                    Message = DialogWindowStrings.DeleteNotFound
                };
            }
            return new ClearOperationResult
            {
                Type = ClearOperationResultType.Success,
                Message = DialogWindowStrings.DeleteSuccess
            };
        }

        private async Task RemoveFiles(List<string> filesToRemove, ObservableCollection<SelectedFile> targetFiles)
        {
            foreach (var file in filesToRemove)
            {
                try
                {
                    //Workaround to prevent System.UnauthorizedAccessException: Access to the path 'pathToFile' is denied. 
                    File.SetAttributes(file, FileAttributes.Normal);

                    await Task.Run(() => File.Delete(file)).ConfigureAwait(true);
                    var removedFile = targetFiles.First(x => x.Path == file);
                    targetFiles.Remove(removedFile);
                }
                catch (Exception ex)
                {
                    _hasErrors = true;
                }

                _filesNotFound = false;
            }
        }
    }
}
