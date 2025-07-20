using Microsoft.EntityFrameworkCore;
using TaskManagement.Domain.Entities;
using TaskManagement.Entities;

namespace TaskManagement.DataAccess;

public class DataContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<TaskItem> Tasks { get; set; }

    public DbSet<AppUser> AppUsers { get; set; }

    public DbSet<TaskLog> TaskLogs { get; set; }

    public DbSet<CategoryLevel> CategoryLevels { get; set; }

    public DbSet<Archive> Archives { get; set; }

    public DbSet<Achievement> Achievements { get; set; }

    public DbSet<Title> Titles { get; set; }

    public DbSet<UserAchievement> UserAchievements { get; set; }

    public DbSet<UserTitle> UserTitles { get; set; }

    public DbSet<User> Users { get; set; }
    public DbSet<UserOTPs> UserOTPs { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<PermissionGroup> PermissionGroups { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<Order> Orders { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // ⚠️ Agar kerakli `OnDelete` yoki `HasKey`, `HasIndex` lar bo‘lsa, shu yerga yoziladi

        builder.ApplyConfigurationsFromAssembly(typeof(DataContext).Assembly);

        builder.Entity<Order>()
            .Property(o => o.Id)
            .ValueGeneratedOnAdd();

        // RolePermission - ko‘p-ko‘p
        builder.Entity<RolePermission>()
            .HasOne(rp => rp.Role)
            .WithMany(r => r.RolePermissions)
            .HasForeignKey(rp => rp.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<RolePermission>()
            .HasOne(rp => rp.Permission)
            .WithMany(p => p.Roles)
            .HasForeignKey(rp => rp.PermissionId)
            .OnDelete(DeleteBehavior.Cascade);

        // UserRole - ko‘p-ko‘p
        builder.Entity<UserRole>()
            .HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<UserRole>()
            .HasOne(ur => ur.Role)
            .WithMany(r => r.Users)
            .HasForeignKey(ur => ur.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        // PermissionGroup - Permission bilan 1-ko‘p
        builder.Entity<Permission>()
            .HasOne(p => p.PermissionGroup)
            .WithMany(pg => pg.Permissions)
            .HasForeignKey(p => p.PermissionGroupId)
            .OnDelete(DeleteBehavior.Cascade);

        // UserOTPs - User bilan 1-ko‘p
        builder.Entity<UserOTPs>()
            .HasOne(uo => uo.User)
            .WithMany() // yoki .WithMany(u => u.OtpCodes) agar navigation bo‘lsa
            .HasForeignKey(uo => uo.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder){ }
}
