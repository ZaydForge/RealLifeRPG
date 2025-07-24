using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Entities;

namespace TaskManagement.Persistence.RepositoryInterfaces;

public interface IUserRepository
{
    Task<UserProfile> GetUserByIdAsync(int userId);
    Task<IEnumerable<UserProfile>> GetAllUsersAsync();
    Task AddUserAsync(UserProfile user);
    Task UpdateUserAsync(UserProfile user);
    Task DeleteUserAsync(UserProfile user);
}
