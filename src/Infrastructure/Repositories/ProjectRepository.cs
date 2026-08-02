using System;
using System.Collections.Generic;
using System.Text;
using Core.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ProjectRepository(ApplicationDbContext context) : IProjectRepository
{
    public async Task<int> AddAsync(ProjectDto dto)
    {
        var project = new Core.Models.Project(dto);
        await context.Projects.AddAsync(project);
        await context.SaveChangesAsync();
        return project.Id;
    }

    public async Task DeleteAsync(int id)
    {
        var project = await context.Projects.FindAsync(id);
        if (project is not null)
        {
            context.Projects.Remove(project);
            await context.SaveChangesAsync();
        }
    }

    public async Task<List<ProjectDto>> GetAllAsync()
    {
        return (await context.Projects.ToListAsync()).Select(x => new ProjectDto(x)).ToList();
    }

    public async Task UpdateAsync(ProjectDto dto)
    {
        var project = await context.Projects.FindAsync(dto.Id);
        if (project is not null)
        {
            project.Update(dto);
            context.Projects.Update(project);
            await context.SaveChangesAsync();
        }
    }
}
