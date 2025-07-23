using TaskManagement.Entities;

namespace TaskManagement.Persistence.RepositoryInterfaces;
public interface ITaskLogRepository
{
    Task AddTaskLogAsync(TaskLog task);
    Task<IEnumerable<TaskLog>> GetTaskLogsAsync();
    Task<TaskLog> GetTaskLogByIdAsync(int id);
    Task DeleteTaskLog(TaskLog taskLog);
    Task SaveChangesAsync();
}
