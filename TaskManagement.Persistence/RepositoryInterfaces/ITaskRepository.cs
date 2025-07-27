using TaskManagement.Entities;

namespace TaskManagement.Persistence.RepositoryInterfaces;
public interface ITaskRepository
{
    Task<IEnumerable<TaskItem>> GetAllAsync();
    Task<TaskItem> GetByIdAsync(int id);
    Task AddAsync(TaskItem task);
    void Update(TaskItem task);
    Task Delete(TaskItem task);
    Task SaveChangesAsync();
    Task<IEnumerable<TaskItem>> GetAllActiveAsync();
}