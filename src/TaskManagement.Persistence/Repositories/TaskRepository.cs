using Microsoft.EntityFrameworkCore;
using TaskManagement.DataAccess;
using TaskManagement.Entities;
using TaskManagement.Persistence.RepositoryInterfaces;

namespace TaskManagement.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly DataContext _context;
    public TaskRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TaskItem>> GetAllAsync()
    {
        return await _context.Tasks
            .ToListAsync();
    }

    public async Task<TaskItem> GetByIdAsync(int id) =>
      await _context.Tasks.FirstOrDefaultAsync(r => r.Id == id);

    public async Task AddAsync(TaskItem task) =>
        await _context.Tasks.AddAsync(task);
   
    public void Update(TaskItem task) =>
        _context.Tasks.Update(task);

    public async Task Delete(TaskItem task) =>
        _context.Tasks.Remove(task);

    public async Task SaveChangesAsync() =>
        await _context.SaveChangesAsync();
    
}
