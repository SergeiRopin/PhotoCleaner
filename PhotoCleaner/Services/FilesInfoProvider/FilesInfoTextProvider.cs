﻿using PhotoCleaner.Domain;
using PhotoCleaner.Localization;
using PhotoCleaner.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace PhotoCleaner.Services.FilesInfoProvider
{
    public class FilesInfoTextProvider : IFilesInfoProvider
    {
        public string GetFilesInfo(IEnumerable<@File> files)
        {
            return files.Any() ? $"{FilesInfoStrings.SelectedFiles}: {files.Count()}." : string.Empty;
        }

        public string GetComparisonInfo(IEnumerable<@File> sourceFiles, IEnumerable<@File> targetFiles)
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