using System;
using System.Collections.Generic;
using System.Text;
using Core.Repositories;

namespace Core.Models;

public class Project
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public DateTime LastOpened { get; set; } = DateTime.Now;

    public Project() { }

    public Project(ProjectDto dto)
    {
        Id = dto.Id;
        Title = dto.Title;
        FilePath = dto.FilePath;
        LastOpened = dto.LastOpened;
    }

    public void Update(ProjectDto dto)
    {
        Title = dto.Title;
        FilePath = dto.FilePath;
        LastOpened = dto.LastOpened;
    }
}
