using System;
using System.Collections.Generic;
using System.Text;
using Core.Models;

namespace Core.Repositories;

public interface IProjectRepository
{
    Task<List<ProjectDto>> GetAllAsync();
    Task<int> AddAsync(ProjectDto dto);
    Task DeleteAsync(int id);
    Task UpdateAsync(ProjectDto dto);
}

public class ProjectDto: Project
{
    public ProjectDto() { }

    public ProjectDto(Project project)
    {
        Id = project.Id;
        Title = project.Title;
        FilePath = project.FilePath;
        LastOpened = project.LastOpened;
    }
}
