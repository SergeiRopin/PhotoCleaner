using PhotoCleaner.App.Domain;
using PhotoCleaner.App.Localization;
using PhotoCleaner.App.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace PhotoCleaner.App.Services.FilesInfoProvider
{
    public class FilesInfoTextProvider : IFilesInfoProvider
    {
        public string GetFilesInfo(IEnumerable<SelectedFile> files)
        {
            return files.Any() ? $"{FilesInfoStrings.SelectedFiles}: {files.Count()}." : string.Empty;
        }

        public string GetComparisonInfo(IEnumerable<SelectedFile> sourceFiles, IEnumerable<SelectedFile> targetFiles)
        {
            if (sourceFiles.Any() && targetFiles.Any())
            {
                var removableFilesCount = Application.Current.Properties[ApplicationConstants.RemovableFilesCount].ToString();

                if (int.Parse(removableFilesCount) == 0)
                    return FilesInfoStrings.NoFilesToRemove;

                return $"{FilesInfoStrings.RemovableFiles}: {removableFilesCount}.";
            }

            return string.Empty;
        }
    }
}
