using Wpf.Other;
using Wpf.Services;

namespace Wpf.Windows.Main
{
    public class MainWindowVM : ViewModel
    {
        private readonly IDialogService _dialogService;
        public MainWindowVM(IDialogService dialogService)
        {
            _dialogService = dialogService;

        }
    }
}
