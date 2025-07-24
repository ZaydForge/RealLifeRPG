using Microsoft.EntityFrameworkCore;
using TaskManagement.DataAccess;
using TaskManagement.Entities;
using TaskManagement.Persistence.RepositoryInterfaces;

namespace TaskManagement.Persistence.Repositories
{
    public class UserRepository : IUserProfileRepository
    {
        private readonly DataContext _context;
        public UserRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<UserProfile> GetUserByIdAsync(int userId)
        {
            return await _context.UserProfiles.FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<IEnumerable<UserProfile>> GetAllUsersAsync()
        {
            return await _context.UserProfiles.ToListAsync();
        }

        public async Task AddUserAsync(UserProfile user)
        {
            await _context.UserProfiles.AddAsync(user);
        }

        public void UpdateUser(UserProfile user)
        {
            _context.UserProfiles.Update(user);
        }

        public void DeleteUser(UserProfile user)
        {
            _context.UserProfiles.Remove(user);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
