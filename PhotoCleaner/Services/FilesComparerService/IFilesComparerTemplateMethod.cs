using PhotoCleaner.Models;
using System.Collections.Generic;

namespace PhotoCleaner.Services.FilesComparerService
{
    public abstract class IFilesComparerTemplateMethod
    {
        public void Compare(IEnumerable<@File> sourceFiles, IEnumerable<@File> targetFiles)
        {
            CleanEmptySpaces(sourceFiles, targetFiles);
            SortFiles(sourceFiles, targetFiles);
            AddEmptySpaces(sourceFiles, targetFiles);
            CleanRemovableFiles(sourceFiles, targetFiles);
            FindRemovableFiles(sourceFiles, targetFiles);
        }

        public abstract void CleanEmptySpaces(IEnumerable<@File> sourceFiles, IEnumerable<@File> targetFiles);
        public abstract void SortFiles(IEnumerable<@File> sourceFiles, IEnumerable<@File> targetFiles);
        public abstract void AddEmptySpaces(IEnumerable<@File> sourceFiles, IEnumerable<@File> targetFiles);
        public abstract void CleanRemovableFiles(IEnumerable<@File> sourceFiles, IEnumerable<@File> targetFiles);
        public abstract void FindRemovableFiles(IEnumerable<@File> sourceFiles, IEnumerable<@File> targetFiles);
    }
}
