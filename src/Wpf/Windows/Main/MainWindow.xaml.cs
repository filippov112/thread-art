using System.Windows;

namespace Wpf.Windows.Main
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainWindowVM _vm;
        public MainWindow(MainWindowVM vm)
        {
            InitializeComponent();
            _vm = vm;
            DataContext = vm;
        }
    }
}
