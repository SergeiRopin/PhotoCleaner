using PhotoCleaner.App.Domain;
using System.Windows;
using System.Windows.Controls;

namespace PhotoCleaner.App.Views
{
    /// <summary>
    /// Interaction logic for Header.xaml
    /// </summary>
    public partial class Header : UserControl
    {
        public static readonly DependencyProperty FilesTypeProperty =
            DependencyProperty.Register("FilesType", typeof(FileType), typeof(UserControl), new PropertyMetadata(FileType.Source));

        public FileType FilesType
        {
            get { return (FileType)GetValue(FilesTypeProperty); }
            set { SetValue(FilesTypeProperty, value); }
        }

        public Header()
        {
            InitializeComponent();
            //DataContext = new HeaderViewModel(FilesType);
        }
    }
}
