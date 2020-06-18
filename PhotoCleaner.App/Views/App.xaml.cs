using PhotoCleaner.App.Domain;
using PhotoCleaner.App.Startup;
using System.Windows;

namespace PhotoCleaner.App.Views
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            UnityConfig.GetConfiguredContainer();
            Application.Current.Properties[ApplicationConstants.CallFromDirectoriesSelector] = true;
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            var container = UnityConfig.GetConfiguredContainer();
            container.Dispose();
        }
    }
}
