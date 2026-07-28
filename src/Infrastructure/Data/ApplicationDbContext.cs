using Application.QueueManager.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<ProcessingJob> Jobs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProcessingJob>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.OriginalSystemPath).IsRequired().HasMaxLength(500);
            entity.Property(e => e.ResultImagePath).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.ResultRoutePath).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.CreatedAt).IsRequired();
        });

        base.OnModelCreating(modelBuilder);
    }
}
