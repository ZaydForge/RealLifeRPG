using Microsoft.EntityFrameworkCore;
using TaskManagement.DataAccess;
using TaskManagement.Entities;
using TaskManagement.Persistence.RepositoryInterfaces;

namespace TaskManagement.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        public UserRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<UserProfile> GetUserByIdAsync(int userId)
        {
            return await _context.AppUsers.FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<IEnumerable<UserProfile>> GetAllUsersAsync()
        {
            return await _context.AppUsers.ToListAsync();
        }

        public async Task AddUserAsync(UserProfile user)
        {
            await _context.AppUsers.AddAsync(user);
        }

        public async Task UpdateUserAsync(UserProfile user)
        {
            _context.AppUsers.Update(user);
        }

        public async Task DeleteUserAsync(UserProfile user)
        {
            _context.AppUsers.Remove(user);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
