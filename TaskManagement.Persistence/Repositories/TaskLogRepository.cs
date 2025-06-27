using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.RepositoryInterfaces;
using TaskManagement.DataAccess;
using TaskManagement.Dtos;
using TaskManagement.Entities;

namespace TaskManagement.Persistence.Repositories
{
    public class TaskLogRepository(DataContext context) : ITaskLogRepository
    {
        public async Task AddTaskLogAsync(TaskLog taskLog)
        {
            await context.TaskLogs.AddAsync(taskLog);

        }

        public async Task DeleteTaskLog(TaskLog taskLog) =>
            context.TaskLogs.Remove(taskLog);

        public async Task<TaskLog> GetTaskLogByIdAsync(int id) =>
            await context.TaskLogs.FirstOrDefaultAsync(r => r.Id == id);

        public async Task<IEnumerable<TaskLog>> GetTaskLogsAsync() =>
            await context.TaskLogs.ToListAsync();

        public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}
