using PhotoCleaner.App.Commands;
using PhotoCleaner.App.Services.DialogService;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using PhotoCleaner.App.Models;
using System;
using System.Linq;
using PhotoCleaner.App.Localization;
using PhotoCleaner.App.Extensions;
using PhotoCleaner.App.Services.FilesComparerService;
using PhotoCleaner.App.Services.FilesInfoProvider;
using PhotoCleaner.App.Domain;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using PhotoCleaner.App.Services.FilesClearStrategy;
using PhotoCleaner.App.Views.DialogWindows;
using PhotoCleaner.Database.Repository;
using System.Threading.Tasks;

namespace PhotoCleaner.App.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private IDialogService _dialogService;
        private IFilesComparerTemplateMethod _filesComparer;
        private IFilesInfoProvider _filesInfoProvider;
        private IDirectoriesRepository _directoryRepo;

        private string _selectedSourceDirectory;
        private string _selectedTargetDirectory;
        private string _selectedSourceExtension;
        private string _selectedTargetExtension;
        private string _lastSourceDirectory;
        private string _lastTargetDirectory;
        private string _sourceFilesInfo;
        private string _targetFilesInfo;
        private string _removableFilesInfo;
        private FilesClearType _filesClearAction;

        public MainWindowViewModel(
            IDialogService dialogService,
            IFilesComparerTemplateMethod filesComparer,
            IFilesInfoProvider filesInfoProvider,
            IDirectoriesRepository directoryRepo)
        {
            _dialogService = dialogService;
            _filesComparer = filesComparer;
            _filesInfoProvider = filesInfoProvider;
            _directoryRepo = directoryRepo;

            if (FileExtensions.Any())
            {
                SelectedSourceExtension = FileExtensions.First();
                SelectedTargetExtension = FileExtensions.First();
            }
            _filesClearAction = FilesClearType.Move;
            SourceLastDirectories = new ObservableCollection<string>(
                Task.Run(async () => await _directoryRepo.GetAllPagedAsync(5, FileType.Source.ToString())).Result);
            TargetLastDirectories = new ObservableCollection<string>(
                Task.Run(async () => await _directoryRepo.GetAllPagedAsync(5, FileType.Target.ToString())).Result);

        }

        public ObservableCollection<SelectedFile> SourceFiles { get; private set; } = new ObservableCollection<SelectedFile>();
        public ObservableCollection<SelectedFile> TargetFiles { get; private set; } = new ObservableCollection<SelectedFile>();
        public ObservableCollection<string> FileExtensions { get; } = new ObservableCollection<string>(Enum.GetNames(typeof(FileExtension)));
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

        public ICommand OpenFilesCommand => new Command(OpenFileDialogCommand);
        
        private async void OpenFileDialogCommand(object obj)
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
                        await UpdateSourceDirectories();
                        break;
                    case FileType.Target:
                        UpdateFiles(TargetFiles);
                        SelectedTargetDirectory = _dialogService.SelectedDirectory;
                        TargetFilesInfo = _filesInfoProvider.GetFilesInfo(TargetFiles);
                        await _directoryRepo.CreateOrUpdateAsync(SelectedTargetDirectory, FileType.Target.ToString());
                        await UpdateTargetDirectories();
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
        }

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

        private void UpdateFiles(ObservableCollection<SelectedFile> files)
        {
            if (files == null)
                files = new ObservableCollection<SelectedFile>();
            files.Clear();
            foreach (var file in _dialogService.SelectedFiles)
            {
                files.Add(file);
            }
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

        private async Task UpdateSourceDirectories()
        {
            await _directoryRepo.CreateOrUpdateAsync(SelectedSourceDirectory, FileType.Source.ToString());
            var sourceDirectories = new ObservableCollection<string>(
                await _directoryRepo.GetAllPagedAsync(5, FileType.Source.ToString()));
            string selectedDirectory = SelectedSourceDirectory;
            SourceLastDirectories.Clear();
            foreach (var item in sourceDirectories)
            {
                SourceLastDirectories.Add(item);
            }
            SelectedSourceDirectory = selectedDirectory;
        }

        private async Task UpdateTargetDirectories()
        {
            await _directoryRepo.CreateOrUpdateAsync(SelectedTargetDirectory, FileType.Target.ToString());
            var targetDirectories = new ObservableCollection<string>(
                await _directoryRepo.GetAllPagedAsync(5, FileType.Target.ToString()));
            string selectedDirectory = SelectedTargetDirectory;
            TargetLastDirectories.Clear();
            foreach (var item in targetDirectories)
            {
                TargetLastDirectories.Add(item);
            }
            SelectedTargetDirectory = selectedDirectory;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
