using System.Collections.ObjectModel;
using System.Windows.Input;
using Core.Repositories;
using Wpf.Other;
using Wpf.Services;
using Wpf.Windows.Main;
using Wpf.Windows.Projects.Models;

namespace Wpf.Windows.Projects
{
    public class ProjectsWindowVM : ViewModel
    {
        private readonly IDialogService _dialogService;
        private readonly IProjectRepository _projectRepository;
        private readonly MainWindowVM _mainWindowVM;

        public event Action? WindowIsClosing;
        public ProjectsWindowVM(IDialogService dialogService, IProjectRepository repository, MainWindowVM mainVM)
        {
            _dialogService = dialogService;
            _projectRepository = repository;

            CreateProjectCommand = new RelayCommand((_) => Task.Run(CreateProject));
            DeleteProjectCommand = new RelayCommand((_) => Task.Run(DeleteProject), () => SelectedProject is not null);
            OpenProjectCommand = new RelayCommand(OpenProject, () => SelectedProject is not null);

            _mainWindowVM = mainVM;
            LoadProjects();
        }

        public ICommand CreateProjectCommand { get; set; }
        public ICommand OpenProjectCommand { get; set; }
        public ICommand DeleteProjectCommand { get; set; }

        public ObservableCollection<ProjectVM> Projects { get; set { field = value; OnPropertyChanged(); } } = [];
        public ProjectVM? SelectedProject { get; set { field = value; OnPropertyChanged(); }  }


        private void LoadProjects()
        {
            Task.Run(async () => {
                try
                {
                    var observableList = new ObservableCollection<ProjectVM>((await _projectRepository.GetAllAsync()).Select(x => new ProjectVM(x)).OrderByDescending(y => y.LastOpened).ToList());
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        Projects = observableList;
                    });
                }
                catch (Exception ex)
                {
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        _dialogService.ShowError($"Ошибка загрузки списка проектов: {ex.Message}");
                    });
                }
            });
        }

        private async Task CreateProject()
        {
            var newProject = new ProjectDto() { Title = "New project" };
            newProject.Id = await _projectRepository.AddAsync(newProject);
            
            App.Current.Dispatcher.Invoke(() => {
                SelectedProject = new(newProject);
                OpenProject(null);
            });
        }

        private async Task DeleteProject()
        {
            await _projectRepository.DeleteAsync(SelectedProject!.Id);
            SelectedProject = null;
            LoadProjects();
        }
        private void OpenProject(object? sender)
        {
            Task.Run(UpdateLastOpenedSelectedProject);
            var main = new MainWindow(_mainWindowVM);
            main.Show();
            _mainWindowVM.OpenProject(SelectedProject!.Id);
            WindowIsClosing?.Invoke();
        }

        private async Task UpdateLastOpenedSelectedProject()
        {
            SelectedProject!.LastOpened = DateTime.Now;
            await _projectRepository.UpdateAsync(SelectedProject.Dto);
        }
    }
}
