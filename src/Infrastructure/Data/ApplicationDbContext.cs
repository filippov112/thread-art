using Core.Models;
using Core.QueueManager.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class ApplicationDbContext: DbContext
{
    public DbSet<ProcessingJob> Jobs { get; set; }
    public DbSet<Project> Projects { get; set; }

    public ApplicationDbContext() 
    {
        Database.EnsureCreated();
        Task.Run(InitialiseDatabaseAsync);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=helloapp.db");
    }

    public async Task InitialiseDatabaseAsync()
    {
        try
        {
            Database.Migrate();
            Console.WriteLine("Миграции успешно применены.");
        }
        catch (Exception ex)
        {
            throw new IOException($"Ошибка при применении миграций: {ex}");
        }
    }
}
