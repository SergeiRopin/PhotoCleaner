using PhotoCleaner.App.Localization;
using PhotoCleaner.App.Services.DialogService;
using PhotoCleaner.App.Services.FilesComparerService;
using PhotoCleaner.App.Services.FilesInfoProvider;
using PhotoCleaner.App.Startup;
using PhotoCleaner.App.ViewModels;
using PhotoCleaner.Database;
using PhotoCleaner.Database.Repository;
using System.Windows;
using System.Windows.Input;
using Unity;

namespace PhotoCleaner.App.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public IUnityContainer Container => UnityConfig.GetConfiguredContainer();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel(
                new DefaultDialogService(), 
                new ClearUniqueFilesComparerTemplateMethod(),
                new FilesInfoTextProvider(),
                Container.Resolve<IDirectoriesRepository>(DBProvider.GetDatabase()));
        }

        private void btnLeftOpen_MouseEnter(object sender, MouseEventArgs e)
        {
            statBarText.Text = StatusBarStrings.OpenSourceFilesMessage;
        }

        private void MouseLeaveArea(object sender, MouseEventArgs e)
        {
            statBarText.Text = StatusBarStrings.ReadyMessage;
        }
    }
}
