using PhotoCleaner.App.Commands;
using PhotoCleaner.App.Services.DialogService;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using PhotoCleaner.App.Models;
using System;
using System.Linq;
using PhotoCleaner.App.Localization;
using PhotoCleaner.App.Services.FilesComparerService;
using PhotoCleaner.App.Services.FilesInfoProvider;
using PhotoCleaner.App.Domain;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using PhotoCleaner.App.Services.FilesClearStrategy;
using PhotoCleaner.App.Views.DialogWindows;
using PhotoCleaner.Database.Repository;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.Generic;
using System.IO;

namespace PhotoCleaner.App.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private IDialogService _dialogService;
        private IFilesComparerTemplateMethod _filesComparer;
        private IFilesInfoProvider _filesInfoProvider;
        private IDirectoriesRepository _directoryRepo;
        private IFileExtensionsRepository _fileExtensionsRepo;
        private IFavouriteExtensionRepository _favouriteExtensionRepo;

        private string _selectedSourceDirectory;
        private string _selectedTargetDirectory;
        private FileExtension _selectedSourceExtension;
        private FileExtension _selectedTargetExtension;
        private string _lastSourceDirectory;
        private string _lastTargetDirectory;
        private string _sourceFilesInfo;
        private string _targetFilesInfo;
        private string _removableFilesInfo;
        private FilesClearType _filesClearAction;

        //TODO: Initialize view model using Unity
        public MainWindowViewModel(
            IDialogService dialogService,
            IFilesComparerTemplateMethod filesComparer,
            IFilesInfoProvider filesInfoProvider,
            IDirectoriesRepository directoryRepo,
            IFileExtensionsRepository fileExtensionsRepo,
            IFavouriteExtensionRepository favouriteExtensionRepo)
        {
            _dialogService = dialogService;
            _filesComparer = filesComparer;
            _filesInfoProvider = filesInfoProvider;
            _directoryRepo = directoryRepo;
            _fileExtensionsRepo = fileExtensionsRepo;
            _favouriteExtensionRepo = favouriteExtensionRepo;

            Initialize();
        }

        public ObservableCollection<SelectedFile> SourceFiles { get; private set; } = new ObservableCollection<SelectedFile>();
        public ObservableCollection<SelectedFile> TargetFiles { get; private set; } = new ObservableCollection<SelectedFile>();
        public ObservableCollection<FileExtension> SourceFileExtensions { get; private set; } = new ObservableCollection<FileExtension>();
        public ObservableCollection<FileExtension> TargetFileExtensions { get; private set; } = new ObservableCollection<FileExtension>();
        public ObservableCollection<string> SourceLastDirectories { get; private set; } = new ObservableCollection<string>();
        public ObservableCollection<string> TargetLastDirectories { get; private set; } = new ObservableCollection<string>();

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
        public FileExtension SelectedSourceExtension
        {
            get => _selectedSourceExtension;
            set
            {
                _selectedSourceExtension = value;
                OnPropertyChanged("SelectedSourceExtension");
                OnPropertyChanged("SelectedSourceFilesMessage");
            }
        }
        public FileExtension SelectedTargetExtension
        {
            get => _selectedTargetExtension;
            set
            {
                _selectedTargetExtension = value;
                OnPropertyChanged("SelectedTargetExtension");
                OnPropertyChanged("SelectedTargetFilesMessage");
            }
        }
        public bool IsSourceFavouriteExtension
        {
            get => SelectedSourceExtension.IsFavourite;
            set
            {
                SelectedSourceExtension.IsFavourite = value;
                OnPropertyChanged("IsSourceFavouriteExtension");
            }
        }
        public bool IsTargetFavouriteExtension
        {
            get => SelectedTargetExtension.IsFavourite;
            set
            {
                SelectedTargetExtension.IsFavourite = value;
                OnPropertyChanged("IsTargetFavouriteExtension");
            }
        }
        public string SelectedSourceFilesMessage => string.Format(SourceFileStrings.SelectFilesMessage, SelectedSourceExtension.Name);
        public string SelectedTargetFilesMessage => string.Format(TargetFileStrings.SelectFilesMessage, SelectedTargetExtension.Name);
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
                OnPropertyChanged("IsActionButtonEnabled");
                OnPropertyChanged("RemovableFilesInfo");
            }
        }
        public bool TargetFilesInfoIsVisible => TargetFilesInfo != null;
        public bool SourceFilesInfoIsVisible => SourceFilesInfo != null;
        public bool IsActionButtonEnabled => RemovableFilesInfo != null && RemovableFilesInfo != FilesInfoStrings.NoFilesToRemove;
        public FilesClearType FilesClearAction
        {
            get => _filesClearAction;
            set
            {
                _filesClearAction = value;
                OnPropertyChanged("ActionButtonTooltip");
                OnPropertyChanged("FilesClearAction");
            }
        }
        public string ActionButtonTooltip => FilesClearAction == FilesClearType.Move ? TooltipStrings.ActionButtonMove : TooltipStrings.ActionButtonDelete;

        public ICommand OpenFilesCommand => new Command(OpenFileDialog);

        public ICommand UpdateClearActionCommand
        {
            get
            {
                return new Command(obj =>
                {
                    FilesClearAction = (FilesClearType)Enum.Parse(typeof(FilesClearType), obj.ToString());
                });
            }
        }

        public ICommand ClearFilesCommand => new Command(ClearFilesUserConsentDialog);

        public ICommand UpdateExtensionCommand => new Command(UpdateExtension);

        public ICommand UpdateFavouriteExtensionCommand => new Command(UpdateFavouriteExtension);

        public ICommand UpdateDirectoryCommand => new Command(OpenFilesFromDirectory);


        private void UpdateExtension(object fileType)
        {
            FileType type = (FileType)Enum.Parse(typeof(FileType), fileType.ToString());
            switch (type)
            {
                case FileType.Source:
                    IsSourceFavouriteExtension = SelectedSourceExtension.IsFavourite;
                    break;
                case FileType.Target:
                    IsTargetFavouriteExtension = SelectedTargetExtension.IsFavourite;
                    break;
            }

            OpenFilesFromDirectory(fileType);
        }

        private async void UpdateFavouriteExtension(object fileType)
        {
            FileType type = (FileType)Enum.Parse(typeof(FileType), fileType.ToString());
            switch (type)
            {
                case FileType.Source:
                    IsSourceFavouriteExtension = SelectedSourceExtension.IsFavourite;

                    if (!SelectedSourceExtension.IsFavourite)
                    {
                        await _favouriteExtensionRepo.Delete(type.ToString());
                        break;
                    }
                    await _favouriteExtensionRepo.CreateOrUpdate(SelectedSourceExtension.Id, type.ToString());

                    foreach (var extension in SourceFileExtensions.Where(x => x.Id != SelectedSourceExtension.Id))
                    {
                        extension.IsFavourite = false;
                    }
                    break;
                case FileType.Target:
                    IsTargetFavouriteExtension = SelectedTargetExtension.IsFavourite;

                    if (!SelectedTargetExtension.IsFavourite)
                    {
                        await _favouriteExtensionRepo.Delete(type.ToString());
                        break;
                    }

                    await _favouriteExtensionRepo.CreateOrUpdate(SelectedTargetExtension.Id, type.ToString());

                    foreach (var extension in TargetFileExtensions.Where(x => x.Id != SelectedTargetExtension.Id))
                    {
                        extension.IsFavourite = false;
                    }
                    break;
            }
        }


        private void Initialize()
        {
            _filesClearAction = FilesClearType.Move;

            SourceLastDirectories = new ObservableCollection<string>(
                Task.Run(async () => await _directoryRepo.GetAllPagedAsync(5, FileType.Source.ToString())).Result);
            TargetLastDirectories = new ObservableCollection<string>(
                Task.Run(async () => await _directoryRepo.GetAllPagedAsync(5, FileType.Target.ToString())).Result);

            var sourceFileExtensions = new ObservableCollection<FileExtension>(
                Task.Run(async () => await _fileExtensionsRepo.GetAllAsync(FileType.Source.ToString())).Result
                .Select(x =>
                    new FileExtension
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Filter = x.Filter,
                        SearchPattern = x.SearchPattern
                    }));

            var targetFileExtensions = new ObservableCollection<FileExtension>(
                Task.Run(async () => await _fileExtensionsRepo.GetAllAsync(FileType.Source.ToString())).Result
                .Select(x =>
                    new FileExtension
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Filter = x.Filter,
                        SearchPattern = x.SearchPattern
                    }));
            SourceFileExtensions = sourceFileExtensions;
            TargetFileExtensions = targetFileExtensions;

            var sourceFavExtension = Task.Run(async () => await _favouriteExtensionRepo.GetByType(FileType.Source.ToString())).Result;
            var targetFavExtension = Task.Run(async () => await _favouriteExtensionRepo.GetByType(FileType.Target.ToString())).Result;

            if (sourceFileExtensions.Any() && sourceFavExtension != null)
            {
                SelectedSourceExtension = sourceFileExtensions.First(x => x.Id == sourceFavExtension.ExtensionId);
                SelectedSourceExtension.IsFavourite = true;
            }
            else
            {
                SelectedSourceExtension = sourceFileExtensions.First();
            }

            if (targetFileExtensions.Any() && targetFavExtension != null)
            {
                SelectedTargetExtension = targetFileExtensions.First(x => x.Id == targetFavExtension.ExtensionId);
                SelectedTargetExtension.IsFavourite = true;
            }
            else
            {
                SelectedTargetExtension = targetFileExtensions.First();
            }
        }

        private void OpenFilesFromDirectory(object obj)
        {
            bool callFromDirSelector = (bool)Application.Current.Properties[ApplicationConstants.CallFromDirectoriesSelector];
            if (!callFromDirSelector)
                return;

            FileType type;
            bool isValidType = Enum.TryParse(obj.ToString(), out type);
            if (!isValidType)
                throw new ArgumentException("Wrong FileType passed as string parameter");

            switch (type)
            {
                case FileType.Source:
                    if (SelectedSourceDirectory == null)
                        return;
                    var sourceDir = Directory.GetFiles(SelectedSourceDirectory, SelectedSourceExtension.SearchPattern);
                    var sourceNewFiles = sourceDir.Select(x =>
                        new SelectedFile
                        {
                            Path = x,
                            Name = Path.GetFileName(x),
                            NameWithoutExtension = Path.GetFileNameWithoutExtension(x),
                            Extension = Path.GetExtension(x)
                        }).ToList();
                    UpdateFiles(SourceFiles, sourceNewFiles);
                    SourceFilesInfo = _filesInfoProvider.GetFilesInfo(SourceFiles);
                    break;
                case FileType.Target:
                    if (SelectedTargetDirectory == null)
                        return;
                    var targetDir = Directory.GetFiles(SelectedTargetDirectory, SelectedTargetExtension.SearchPattern);
                    var targetNewFiles = targetDir.Select(x =>
                        new SelectedFile
                        {
                            Path = x,
                            Name = Path.GetFileName(x),
                            NameWithoutExtension = Path.GetFileNameWithoutExtension(x),
                            Extension = Path.GetExtension(x)
                        }).ToList();
                    UpdateFiles(TargetFiles, targetNewFiles);
                    TargetFilesInfo = _filesInfoProvider.GetFilesInfo(TargetFiles);
                    break;
            }

            if (SourceFiles.Any() && TargetFiles.Any())
            {
                _filesComparer.Compare(SourceFiles, TargetFiles);
                RemovableFilesInfo = _filesInfoProvider.GetComparisonInfo(SourceFiles, TargetFiles);
            }
        }

        private async void OpenFileDialog(object obj)
        {
            Application.Current.Properties[ApplicationConstants.CallFromDirectoriesSelector] = false;

            FileType type = (FileType)Enum.Parse(typeof(FileType), obj.ToString());
            if (CanOpenFileDialog(type))
            {
                switch (type)
                {
                    case FileType.Source:
                        UpdateFiles(SourceFiles, _dialogService.SelectedFiles);
                        SelectedSourceDirectory = _dialogService.SelectedDirectory;
                        SourceFilesInfo = _filesInfoProvider.GetFilesInfo(SourceFiles);
                        await UpdateDirectories(FileType.Source);
                        break;
                    case FileType.Target:
                        UpdateFiles(TargetFiles, _dialogService.SelectedFiles);
                        SelectedTargetDirectory = _dialogService.SelectedDirectory;
                        TargetFilesInfo = _filesInfoProvider.GetFilesInfo(TargetFiles);
                        await UpdateDirectories(FileType.Target);
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

            Application.Current.Properties[ApplicationConstants.CallFromDirectoriesSelector] = true;
        }

        private async void ClearFilesUserConsentDialog(object o)
        {
            string clearFilesDialogMessage = string.Format(DialogWindowStrings.ClearFilesDialogMessage,
                FilesClearAction == FilesClearType.Move
                ? DialogWindowStrings.Move.ToLower()
                : DialogWindowStrings.Delete.ToLower());

            var view = new UserConsentDialogContent
            {
                DataContext = new UserConsentDialogViewModel(clearFilesDialogMessage)
            };
            await DialogHost.Show(view, "RootDialog", ClearFilesDialogClosingEventHandler);
        }

        private async void ClearFilesDialogClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            if (eventArgs.Parameter == null || (bool)eventArgs.Parameter == false)
                return;

            //lets cancel the close...
            eventArgs.Cancel();

            //update the "session" with progress bar content
            ((DialogHost)eventArgs.OriginalSource).CloseOnClickAway = false;
            eventArgs.Session.UpdateContent(new ProgressBarDialogContent());

            var filesClearType = (FilesClearType)Enum.Parse(typeof(FilesClearType), FilesClearAction.ToString());
            IFilesClearStrategy filesClearStrategy = null;
            ClearOperationResult result = null;
            switch (filesClearType)
            {
                case FilesClearType.Move:
                    filesClearStrategy = new FilesMoveStrategy(this);
                    result = await filesClearStrategy.ClearFiles();
                    break;
                case FilesClearType.Delete:
                    filesClearStrategy = new FilesDeleteStrategy(this);
                    result = await filesClearStrategy.ClearFiles();
                    break;
            }

            if (SourceFiles.Any() && TargetFiles.Any())
            {
                _filesComparer.Compare(SourceFiles, TargetFiles);
                TargetFilesInfo = _filesInfoProvider.GetFilesInfo(TargetFiles);
                RemovableFilesInfo = _filesInfoProvider.GetComparisonInfo(SourceFiles, TargetFiles);
            }

            ((DialogHost)eventArgs.OriginalSource).CloseOnClickAway = true;
            var view = new InfoDialogContent
            {
                DataContext = new InfoDialogViewModel(result.Message, result.Type)
            };
            eventArgs.Session.UpdateContent(view);
        }

        private bool CanOpenFileDialog(FileType type)
        {
            switch (type)
            {
                case FileType.Source:
                    return _dialogService.OpenFileDialog(SelectedSourceExtension.Filter, _lastSourceDirectory);
                case FileType.Target:
                    return _dialogService.OpenFileDialog(SelectedTargetExtension.Filter, _lastTargetDirectory);
                default:
                    return false;
            }
        }

        private static void UpdateFiles(ObservableCollection<SelectedFile> oldFiles, IEnumerable<SelectedFile> newFiles)
        {
            if (oldFiles == null)
                oldFiles = new ObservableCollection<SelectedFile>();
            oldFiles.Clear();
            foreach (var file in newFiles)
            {
                oldFiles.Add(file);
            }
        }

        private async Task UpdateDirectories(FileType type)
        {
            switch (type)
            {
                case FileType.Source:
                    if (SourceLastDirectories.Contains(SelectedSourceDirectory))
                        return;

                    await _directoryRepo.CreateOrUpdateAsync(SelectedSourceDirectory, type.ToString());
                    var sourceDirectories = new ObservableCollection<string>(
                        await _directoryRepo.GetAllPagedAsync(5, type.ToString()));
                    string selectedSourceDir = SelectedSourceDirectory;
                    SourceLastDirectories.Clear();
                    foreach (var item in sourceDirectories)
                    {
                        SourceLastDirectories.Add(item);
                    }
                    SelectedSourceDirectory = selectedSourceDir;
                    break;
                case FileType.Target:
                    if (TargetLastDirectories.Contains(SelectedTargetDirectory))
                        return;

                    await _directoryRepo.CreateOrUpdateAsync(SelectedTargetDirectory, FileType.Target.ToString());
                    var targetDirectories = new ObservableCollection<string>(
                        await _directoryRepo.GetAllPagedAsync(5, FileType.Target.ToString()));
                    string selectedTargetDir = SelectedTargetDirectory;
                    TargetLastDirectories.Clear();
                    foreach (var item in targetDirectories)
                    {
                        TargetLastDirectories.Add(item);
                    }
                    SelectedTargetDirectory = selectedTargetDir;
                    break;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
