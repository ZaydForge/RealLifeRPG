using Microsoft.EntityFrameworkCore;
using System;
using TaskManagement.DataAccess;
using TaskManagement.Domain.Enums;
using TaskManagement.Persistence.RepositoryInterfaces;
using TaskManagement.Entities;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Repositories;

public class CategoryLevelRepository : ICategoryLevelRepository
{
    private readonly DataContext _context;

    public CategoryLevelRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<UserCategory>> GetAllAsync() =>
        await _context.UserCategories
            .Include(uc => uc.Category)
            .ToListAsync();

    public async Task<UserCategory> GetByIdAsync(int id) =>
        await _context.UserCategories
            .FirstOrDefaultAsync(cl => cl.Id == id);

    public async Task<UserCategory> GetByCategoryAsync(CategoryName category) =>
    await _context.UserCategories
        .FirstOrDefaultAsync(cl => cl.Category.CategoryName == category);

    public async Task<UserCategory> GetByCategoryAsync(CategoryName category, int userId) =>
        await _context.UserCategories
            .FirstOrDefaultAsync(cl => cl.Category.CategoryName == category && cl.UserId == userId);

    public void Update(UserCategory category) =>
        _context.UserCategories.Update(category);

    public async Task SaveChangesAsync() =>
        await _context.SaveChangesAsync();
}

