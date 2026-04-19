using Domain.Models;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }
    public DbSet<ProcessedResult> ProcessedResults { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProcessedResult>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.OriginalFilePath).IsRequired().HasMaxLength(500);
            entity.Property(e => e.ResultImagePath).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.ResultRoutePath).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.CreatedAt).IsRequired();
        });

        base.OnModelCreating(modelBuilder);
    }
}
