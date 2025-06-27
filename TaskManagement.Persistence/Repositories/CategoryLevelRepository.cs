using Microsoft.EntityFrameworkCore;
using System;
using TaskManagement.DataAccess;
using TaskManagement.Domain.Enums;
using TaskManagement.Domain.Interfaces;
using TaskManagement.Entities;

namespace TaskManagement.Repositories;

public class CategoryLevelRepository : ICategoryLevelRepository
{
    private readonly DataContext _context;

    public CategoryLevelRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CategoryLevel>> GetAllAsync() =>
        await _context.CategoryLevels
        .ToListAsync();

    public async Task<CategoryLevel> GetByIdAsync(int id) =>
        await _context.CategoryLevels
            .FirstOrDefaultAsync(cl => cl.Id == id);

    public async Task<CategoryLevel> GetByCategoryAsync(Category category) =>
    await _context.CategoryLevels
        .FirstOrDefaultAsync(cl => cl.Category == category);

    public void Update(CategoryLevel level) =>
        _context.CategoryLevels.Update(level);

    public async Task SaveChangesAsync() =>
        await _context.SaveChangesAsync();
}

