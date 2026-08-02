using System;
using System.Collections.Generic;
using System.Text;
using Core.Repositories;
using Wpf.Other;

namespace Wpf.Windows.Projects.Models;

public class ProjectVM: ViewModel
{
    public ProjectDto Dto { get; private set; }
    public ProjectVM(ProjectDto dto)
    {
        Dto = dto;
    }

    public string Title { get => Dto.Title; set { Dto.Title = value; OnPropertyChanged(); } }
    public string FilePath { get => Dto.FilePath; set { Dto.FilePath = value; OnPropertyChanged(); } }
    public DateTime LastOpened { get => Dto.LastOpened; set { Dto.LastOpened = value; OnPropertyChanged(); } }
    public int Id { get => Dto.Id; set { Dto.Id = value; OnPropertyChanged(); } }
}
