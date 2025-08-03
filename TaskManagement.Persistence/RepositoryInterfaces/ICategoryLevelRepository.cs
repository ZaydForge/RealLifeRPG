using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Persistence.RepositoryInterfaces;

public interface ICategoryLevelRepository
{
    Task<IEnumerable<UserCategory>> GetAllAsync();
    Task<UserCategory> GetByIdAsync(int id);
    Task<UserCategory> GetByCategoryAsync(CategoryName category);
    Task<UserCategory> GetByCategoryAsync(CategoryName category, int userId);
    void Update(UserCategory level);
    Task SaveChangesAsync();
}