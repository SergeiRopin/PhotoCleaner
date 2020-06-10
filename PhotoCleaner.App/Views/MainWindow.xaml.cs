using PhotoCleaner.App.Localization;
using PhotoCleaner.App.Services.DialogService;
using PhotoCleaner.App.Services.FilesComparerService;
using PhotoCleaner.App.Services.FilesInfoProvider;
using PhotoCleaner.App.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace PhotoCleaner.App.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel(
                new DefaultDialogService(), 
                new ClearUniqueFilesComparerTemplateMethod(),
                new FilesInfoTextProvider());
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
