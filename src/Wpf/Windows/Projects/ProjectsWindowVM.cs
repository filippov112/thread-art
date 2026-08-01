using Wpf.Other;
using Wpf.Services;

namespace Wpf.Windows.Projects
{
    public class ProjectsWindowVM : ViewModel
    {
        private readonly IDialogService _dialogService;
        public ProjectsWindowVM(IDialogService dialogService)
        {
            _dialogService = dialogService;

        }
    }
}
