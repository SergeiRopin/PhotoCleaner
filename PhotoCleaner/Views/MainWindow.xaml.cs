using PhotoCleaner.Localization;
using PhotoCleaner.Services.DialogService;
using PhotoCleaner.Services.FilesComparerService;
using PhotoCleaner.Services.FilesInfoProvider;
using PhotoCleaner.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace PhotoCleaner.Views
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
                new RemoveUniqueFilesComparerTemplateMethod(),
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
