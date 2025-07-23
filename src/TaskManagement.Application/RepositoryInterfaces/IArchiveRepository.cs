using TaskManagement.Entities;

namespace TaskManagement.Domain.Interfaces;

public interface IArchiveRepository
{
    Task<IEnumerable<Archive>> GetAllAsync();
    Task<Archive> GetByIdAsync(int id);
    Task AddAsync(Archive archive);
    void Delete(Archive archive);
    Task SaveChangesAsync();
}