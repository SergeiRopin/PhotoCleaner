using PhotoCleaner.App.Models;
using System.Collections.Generic;

namespace PhotoCleaner.App.Services.FilesComparerService
{
    public abstract class IFilesComparerTemplateMethod
    {
        public void Compare(IEnumerable<SelectedFile> sourceFiles, IEnumerable<SelectedFile> targetFiles)
        {
            CleanEmptySpaces(sourceFiles, targetFiles);
            SortFiles(sourceFiles, targetFiles);
            AddEmptySpaces(sourceFiles, targetFiles);
            CleanRemovableFiles(sourceFiles, targetFiles);
            FindRemovableFiles(sourceFiles, targetFiles);
        }

        public abstract void CleanEmptySpaces(IEnumerable<SelectedFile> sourceFiles, IEnumerable<SelectedFile> targetFiles);
        public abstract void SortFiles(IEnumerable<SelectedFile> sourceFiles, IEnumerable<SelectedFile> targetFiles);
        public abstract void AddEmptySpaces(IEnumerable<SelectedFile> sourceFiles, IEnumerable<SelectedFile> targetFiles);
        public abstract void CleanRemovableFiles(IEnumerable<SelectedFile> sourceFiles, IEnumerable<SelectedFile> targetFiles);
        public abstract void FindRemovableFiles(IEnumerable<SelectedFile> sourceFiles, IEnumerable<SelectedFile> targetFiles);
    }
}
