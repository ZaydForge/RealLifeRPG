using TaskManagement.Entities;

namespace TaskManagement.Persistence.RepositoryInterfaces;
public interface IArchiveRepository
{
    Task<IEnumerable<Archive>> GetAllAsync();
    Task<Archive> GetByIdAsync(int id);
    Task AddAsync(Archive archive);
    void Delete(Archive archive);
    Task SaveChangesAsync();
}