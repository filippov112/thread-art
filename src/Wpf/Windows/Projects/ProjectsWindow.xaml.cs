using System.Windows;

namespace Wpf.Windows.Projects
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ProjectsWindow : Window
    {
        private readonly ProjectsWindowVM _vm;
        public ProjectsWindow(ProjectsWindowVM vm)
        {
            InitializeComponent();
            _vm = vm;
            DataContext = vm;

            MouseLeftButtonDown += ProjectsWindow_MouseLeftButtonDown;
        }

        private void ProjectsWindow_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
