using PhotoCleaner.Domain;
using PhotoCleaner.Localization;
using PhotoCleaner.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PhotoCleaner.Services.FilesClearStrategy
{
    public class FilesMoveStrategy : IFilesClearStrategy
    {
        INotifyPropertyChanged _viewModel;
        bool _hasErrors = false;
        bool _filesNotFound = true;

        public FilesMoveStrategy(INotifyPropertyChanged viewModel)
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

            var namesToMove = targetFiles.Except(sourceFiles).ToArray();

            var filesToMove = new List<string>();
            var targetFileExtension = mainViewModel.TargetFiles.First(x => !string.IsNullOrEmpty(x.NameWithoutExtension)).Extension;
            filesToMove.AddRange(
                namesToMove.Select(name =>
                Path.Combine(mainViewModel.SelectedTargetDirectory, name) + targetFileExtension));

            var parentDir = Directory.GetParent(mainViewModel.SelectedTargetDirectory).FullName;
            var currentDir = new DirectoryInfo(mainViewModel.SelectedTargetDirectory).Name;
            var directoryToMove = Directory.CreateDirectory(Path.Combine(parentDir, $"{currentDir}.ClearedFiles")).FullName;

            await MoveFiles(filesToMove, directoryToMove, mainViewModel.TargetFiles).ConfigureAwait(true);

            if (_hasErrors)
            {
                return new ClearOperationResult
                {
                    Type = ClearOperationResultType.Fail,
                    Message = DialogWindowStrings.MoveFail
                };
            }
            if (!_hasErrors && _filesNotFound)
            {
                return new ClearOperationResult
                {
                    Type = ClearOperationResultType.NotFound,
                    Message = DialogWindowStrings.MoveNotFound
                };
            }
            return new ClearOperationResult
            {
                Type = ClearOperationResultType.Success,
                Message = string.Format(DialogWindowStrings.MoveSuccess, directoryToMove)
            };
        }

        private async Task MoveFiles(List<string> filesToMove, string directoryToMove, ObservableCollection<Models.File> targetFiles)
        {
            foreach (var srcFile in filesToMove)
            {
                try
                {
                    //Workaround to prevent System.UnauthorizedAccessException: Access to the path 'pathToFile' is denied. 
                    File.SetAttributes(srcFile, FileAttributes.Normal);

                    string destFileName = $"{directoryToMove}\\{Path.GetFileName(srcFile)}";
                    await Task.Run(() => File.Move(srcFile, destFileName)).ConfigureAwait(true);
                    var movedFile = targetFiles.First(x => x.Path == srcFile);
                    targetFiles.Remove(movedFile);
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
