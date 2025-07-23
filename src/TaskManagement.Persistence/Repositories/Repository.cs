using Microsoft.EntityFrameworkCore;
using TaskManagement.DataAccess;
using TaskManagement.Persistence.RepositoryInterfaces;

namespace TaskManagement.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly DataContext _context;
    private readonly DbSet<T> _dbSet;

    public Repository(DataContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);

    public void Delete(T entity) => _dbSet.Remove(entity);

    public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();

    public async Task<T> GetByIdAsync(int id) => await _dbSet.FindAsync(id);

    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();

    public void Update(T entity) => _dbSet.Update(entity);
}   
