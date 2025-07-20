using TaskManagement.Domain.Enums;
using TaskManagement.Entities;

namespace TaskManagement.Persistence.RepositoryInterfaces;

public interface ICategoryLevelRepository
{
    Task<IEnumerable<CategoryLevel>> GetAllAsync();
    Task<CategoryLevel> GetByIdAsync(int id);
    Task<CategoryLevel> GetByCategoryAsync(Category category);
    void Update(CategoryLevel level);
    Task SaveChangesAsync();
}