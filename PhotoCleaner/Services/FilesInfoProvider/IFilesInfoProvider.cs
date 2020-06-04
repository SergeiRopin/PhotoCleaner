using PhotoCleaner.Models;
using System.Collections.Generic;

namespace PhotoCleaner.Services.FilesInfoProvider
{
    public interface IFilesInfoProvider
    {
        string GetFilesInfo(IEnumerable<@File> files);
        string GetComparisonInfo(IEnumerable<@File> sourceFiles, IEnumerable<@File> targetFiles);
    }
}
