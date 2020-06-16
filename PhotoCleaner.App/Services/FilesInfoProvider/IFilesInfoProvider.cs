using PhotoCleaner.App.Models;
using System.Collections.Generic;

namespace PhotoCleaner.App.Services.FilesInfoProvider
{
    public interface IFilesInfoProvider
    {
        string GetFilesInfo(IEnumerable<SelectedFile> files);
        string GetComparisonInfo(IEnumerable<SelectedFile> sourceFiles, IEnumerable<SelectedFile> targetFiles);
    }
}
