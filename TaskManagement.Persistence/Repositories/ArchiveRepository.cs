using Microsoft.EntityFrameworkCore;
using TaskManagement.DataAccess;
using TaskManagement.Domain.Interfaces;
using TaskManagement.Entities;

namespace TaskManagement.Repositories;

public class ArchiveRepository : IArchiveRepository
{
    private readonly DataContext _context;

    public ArchiveRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Archive>> GetAllAsync() =>
        await _context.Archives.ToListAsync();

    public async Task<Archive> GetByIdAsync(int id) =>
        await _context.Archives.FirstOrDefaultAsync(r => r.Id == id);

    public async Task AddAsync(Archive archive) =>
        await _context.Archives.AddAsync(archive);

    public void Delete(Archive archive) =>
        _context.Archives.Remove(archive);

    public async Task SaveChangesAsync() =>
        await _context.SaveChangesAsync();
}