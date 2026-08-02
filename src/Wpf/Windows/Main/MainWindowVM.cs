using System;
using System.Collections.Generic;
using System.Text;
using Wpf.Other;
using Wpf.Services;

namespace Wpf.Windows.Main;

public class MainWindowVM : ViewModel
{
    private readonly IDialogService _dialogService;

    public MainWindowVM(IDialogService dialogService)
    {
        _dialogService = dialogService;
    }

    public void OpenProject(int id)
    {
        //_dialogService.ShowWarning($"Открыт проект:{id}");
    }
}
