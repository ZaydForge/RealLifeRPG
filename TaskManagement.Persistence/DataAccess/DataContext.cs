using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TaskManagement.Entities;

namespace TaskManagement.DataAccess;

public class DataContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<TaskItem> Tasks { get; set; }
    public DbSet<AppUser> Users { get; set; }
    public DbSet<TaskLog> TaskLogs { get; set; }
    public DbSet<CategoryLevel> CategoryLevels { get; set; }
    public DbSet<Archive> Archives { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DataContext).Assembly);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder){ }
}
