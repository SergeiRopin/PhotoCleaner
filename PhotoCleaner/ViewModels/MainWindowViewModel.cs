using PhotoCleaner.Commands;
using PhotoCleaner.Services.DialogService;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using PhotoCleaner.Models;
using System;
using System.Linq;
using PhotoCleaner.Localization;
using PhotoCleaner.Extensions;
using PhotoCleaner.Services.FilesComparerService;
using PhotoCleaner.Services.FilesInfoProvider;
using PhotoCleaner.Domain;

namespace PhotoCleaner.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private IDialogService _dialogService;
        private IFilesComparerTemplateMethod _filesComparer;
        private IFilesInfoProvider _filesInfoProvider;

        private string _selectedSourceDirectory;
        private string _selectedTargetDirectory;
        private string _selectedSourceExtension;
        private string _selectedTargetExtension;
        private string _lastSourceDirectory;
        private string _lastTargetDirectory;
        private string _sourceFilesInfo;
        private string _targetFilesInfo;
        private string _removableFilesInfo;

        public MainWindowViewModel(
            IDialogService dialogService, 
            IFilesComparerTemplateMethod filesComparer, 
            IFilesInfoProvider filesInfoProvider)
        {
            _dialogService = dialogService;
            _filesComparer = filesComparer;
            _filesInfoProvider = filesInfoProvider;

            if (FileExtensions.Any())
            {
                SelectedSourceExtension = FileExtensions.First();
                SelectedTargetExtension = FileExtensions.First();
            }
        }

        public ObservableCollection<@File> SourceFiles { get; private set; } = new ObservableCollection<@File>();
        public ObservableCollection<@File> TargetFiles { get; private set; } = new ObservableCollection<@File>();
        public ObservableCollection<string> FileExtensions { get; } = new ObservableCollection<string>(Enum.GetNames(typeof(FileExtension)));

        public string SelectedSourceDirectory
        {
            get => _selectedSourceDirectory;
            set
            {
                _selectedSourceDirectory = value;
                _lastSourceDirectory = _selectedSourceDirectory;
                OnPropertyChanged("SelectedSourceDirectory");
            }
        }
        public string SelectedTargetDirectory
        {
            get => _selectedTargetDirectory;
            set
            {
                _selectedTargetDirectory = value;
                _lastTargetDirectory = _selectedTargetDirectory;
                OnPropertyChanged("SelectedTargetDirectory");
            }
        }
        public string SelectedSourceExtension
        {
            get => _selectedSourceExtension;
            set
            {
                _selectedSourceExtension = value;
                OnPropertyChanged("SelectedSourceExtension");
                OnPropertyChanged("SelectedSourceFilesMessage");
            }
        }
        public string SelectedTargetExtension
        {
            get => _selectedTargetExtension;
            set
            {
                _selectedTargetExtension = value;
                OnPropertyChanged("SelectedTargetExtension");
                OnPropertyChanged("SelectedTargetFilesMessage");
            }
        }
        public string SelectedSourceFilesMessage => string.Format(SourceFileStrings.SelectFilesMessage, SelectedSourceExtension);
        public string SelectedTargetFilesMessage => string.Format(TargetFileStrings.SelectFilesMessage, SelectedTargetExtension);
        public string SourceFilesInfo
        {
            get => _sourceFilesInfo;
            set
            {
                _sourceFilesInfo = value;
                OnPropertyChanged("SourceFilesInfo");
                OnPropertyChanged("SourceFilesInfoIsVisible");
            }
        }
        public string TargetFilesInfo
        {
            get => _targetFilesInfo;
            set
            {
                _targetFilesInfo = value;
                OnPropertyChanged("TargetFilesInfo");
                OnPropertyChanged("TargetFilesInfoIsVisible");
            }
        }
        public string RemovableFilesInfo
        {
            get => _removableFilesInfo;
            set
            {
                _removableFilesInfo = value;
                OnPropertyChanged("RemovableFilesInfo");
            }
        }
        public bool TargetFilesInfoIsVisible => TargetFilesInfo != null;
        public bool SourceFilesInfoIsVisible => SourceFilesInfo != null;

        public Command OpenFilesCommand
        {
            get
            {
                return new Command(obj =>
                {
                    FileType type = (FileType)Enum.Parse(typeof(FileType), obj.ToString());
                    if (CanOpenFileDialog(type))
                    {
                        switch (type)
                        {
                            case FileType.Source:
                                UpdateFiles(SourceFiles);
                                SelectedSourceDirectory = _dialogService.SelectedDirectory;
                                SourceFilesInfo = _filesInfoProvider.GetFilesInfo(SourceFiles);
                                break;
                            case FileType.Target:
                                UpdateFiles(TargetFiles);
                                SelectedTargetDirectory = _dialogService.SelectedDirectory;
                                TargetFilesInfo = _filesInfoProvider.GetFilesInfo(TargetFiles);
                                break;
                            default:
                                break;
                        }
                    }

                    if (SourceFiles.Any() && TargetFiles.Any())
                    {
                        _filesComparer.Compare(SourceFiles, TargetFiles);
                        RemovableFilesInfo = _filesInfoProvider.GetComparisonInfo(SourceFiles, TargetFiles);
                    }
                });
            }
        }

        private bool CanOpenFileDialog(FileType type)
        {
            switch (type)
            {
                case FileType.Source:
                    var sourceExtension = ((FileExtension)Enum.Parse(typeof(FileExtension), SelectedSourceExtension)).ToDescription();
                    return _dialogService.OpenFileDialog(sourceExtension, _lastSourceDirectory);
                case FileType.Target:
                    var targetExtension = ((FileExtension)Enum.Parse(typeof(FileExtension), SelectedTargetExtension)).ToDescription();
                    return _dialogService.OpenFileDialog(targetExtension, _lastTargetDirectory);
                default:
                    return false;
            }
        }

        private void UpdateFiles(ObservableCollection<@File> files)
        {
            if (files == null)
                files = new ObservableCollection<File>();
            files.Clear();
            foreach (var file in _dialogService.SelectedFiles)
            {
                files.Add(file);
            }
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
