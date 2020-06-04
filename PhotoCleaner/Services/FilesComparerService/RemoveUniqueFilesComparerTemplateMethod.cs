using PhotoCleaner.Domain;
using PhotoCleaner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace PhotoCleaner.Services.FilesComparerService
{
    public class RemoveUniqueFilesComparerTemplateMethod : IFilesComparerTemplateMethod
    {
        private IEnumerable<File> _filesToRemove;
        public override void CleanEmptySpaces(IEnumerable<@File> sourceFiles, IEnumerable<@File> targetFiles)
        {
            var source = sourceFiles as IList<@File>;
            var target = targetFiles as IList<@File>;

            if (source != null)
            {
                foreach (var file in source.Where(x => x.IsEmpty).ToList())
                {
                    source.Remove(file);
                }
            }
            if (target != null)
            {
                foreach (var file in target.Where(x => x.IsEmpty).ToList())
                {
                    target.Remove(file);
                }
            }
        }

        public override void SortFiles(IEnumerable<File> sourceFiles, IEnumerable<File> targetFiles)
        {
            sourceFiles.OrderBy(x => x.NameWithoutExtension);
            targetFiles.OrderBy(x => x.NameWithoutExtension);
        }

        public override void AddEmptySpaces(IEnumerable<@File> sourceFiles, IEnumerable<@File> targetFiles)
        {
            var source = sourceFiles as IList<@File>;
            var target = targetFiles as IList<@File>;

            if (source != null && target != null)
            {
                var emptySpace = new @File { Name = string.Empty, IsEmpty = true };
                int filesCount = target.Count > source.Count ? target.Count : source.Count;
                int iteration = 1;

                for (int i = 0; i < filesCount; i++)
                {
                    if (source.Count > i && target.Count > i && target[i].NameWithoutExtension != source[i].NameWithoutExtension)
                    {
                        if (source.Any(x => x.NameWithoutExtension == target[i].NameWithoutExtension))
                            target.Insert(i, emptySpace);
                        else
                        {
                            if (string.Compare(source[i].NameWithoutExtension, target[i].NameWithoutExtension, StringComparison.InvariantCultureIgnoreCase) > 0)
                                source.Insert(i, emptySpace);
                            else if (string.Compare(source[i].NameWithoutExtension, target[i].NameWithoutExtension, StringComparison.InvariantCultureIgnoreCase) < 0)
                                target.Insert(i, emptySpace);
                        }
                    }
                    else if (target.Count <= i)
                        target.Add(emptySpace);
                    else if (source.Count <= i)
                        source.Add(emptySpace);

                    //Handle last empty space
                    if (iteration == filesCount)
                    {
                        if (source.Count + 1 == target.Count)
                            source.Add(emptySpace);
                        else if (source.Count == target.Count + 1)
                            target.Add(emptySpace);
                    }

                    iteration++;
                }
            }
        }

        public override void CleanRemovableFiles(IEnumerable<@File> sourceFiles, IEnumerable<@File> targetFiles)
        {
            _filesToRemove = targetFiles
                .Where(x => !sourceFiles.Select(y => y.NameWithoutExtension).Contains(x.NameWithoutExtension))
                .Where(x => !x.IsEmpty);

            Application.Current.Properties[ApplicationConstants.RemovableFilesCount] = _filesToRemove.Count();

            var nonRemovableFiles = targetFiles.Except(_filesToRemove);
            foreach (var file in nonRemovableFiles)
            {
                file.IsUnique = false;
            }
        }

        public override void FindRemovableFiles(IEnumerable<@File> sourceFiles, IEnumerable<@File> targetFiles)
        {
            foreach (var file in _filesToRemove)
            {
                file.IsUnique = true;
            }
        }
    }
}
