using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskManagement.DataAccess;
using TaskManagement.Domain.Entities;
using TaskManagement.Persistence.Repositories;
using TaskManagement.Persistence.RepositoryInterfaces;
using TaskManagement.Repositories;

namespace TaskManagement.Persistence
{
    public static class PersistenceServiceRegistration
    {
        public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DataContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
                .UseSnakeCaseNamingConvention()
                .LogTo(Console.Write, Microsoft.Extensions.Logging.LogLevel.Information));

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<ITaskRepository, TaskRepository>();
            services.AddScoped<ITaskLogRepository, TaskLogRepository>();
            services.AddScoped<IArchiveRepository, ArchiveRepository>();
            services.AddScoped<ICategoryLevelRepository, CategoryLevelRepository>();
            services.AddScoped<IUserProfileRepository, UserRepository>();
            services.AddScoped<IAchievementRepository, AchievementRepository>();
            

            return services;
        }

        // ⚙️ Boshlang'ich admin rol va permissionsni qo'shish uchun method
        public static async Task SeedAdminRolePermissionsAsync(DataContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            // Admin roli ID
            const int AdminRoleId = 1;
            //const int SuperAdminUserId = 1;

            // SuperAdmin oldin biriktirilganmi tekshiramiz
            var hasAdminRole = await context.UserRoles
                .AnyAsync(ur => ur.RoleId == AdminRoleId);

            if (!hasAdminRole)
            {
                context.UserRoles.Add(new UserRole
                {
                    RoleId = AdminRoleId,
                    UserId = 1
                });
                await context.SaveChangesAsync();
            }

            // Admin roliga biriktirilmagan permissionlar topiladi
            var missingPermissions = await context.Permissions
                .Where(p => !context.RolePermissions
                    .Where(rp => rp.RoleId == AdminRoleId)
                    .Select(rp => rp.PermissionId)
                    .Contains(p.Id))
                .ToListAsync();

            if (missingPermissions.Any())
            {
                var newRolePermissions = missingPermissions.Select(p =>
                    new RolePermission
                    {
                        RoleId = AdminRoleId,
                        PermissionId = p.Id
                    });

                await context.RolePermissions.AddRangeAsync(newRolePermissions);
                await context.SaveChangesAsync();
            }
        }
    }
}
