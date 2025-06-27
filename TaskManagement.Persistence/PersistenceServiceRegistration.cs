using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Application.RepositoryInterfaces;
using TaskManagement.DataAccess;
using TaskManagement.Domain.Interfaces;
using TaskManagement.Persistence.Repositories;
using TaskManagement.Repositories;

namespace TaskManagement.Persistence
{
    public static class PersistenceServiceRegistration
    {
        public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DataContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
                .UseSnakeCaseNamingConvention());

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<ITaskRepository, TaskRepository>();
            services.AddScoped<ITaskLogRepository, TaskLogRepository>();
            services.AddScoped<IArchiveRepository, ArchiveRepository>();
            services.AddScoped<ICategoryLevelRepository, CategoryLevelRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            return services;
        }
    }
}
