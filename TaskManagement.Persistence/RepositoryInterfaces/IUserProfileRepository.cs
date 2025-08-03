using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Entities;

namespace TaskManagement.Persistence.RepositoryInterfaces;

public interface IUserProfileRepository
{
    Task<UserProfile> GetByUserIdAsync(int? userId);
    Task<UserProfile> GetProfileByIdAsync(int profileId);
    Task<IEnumerable<UserProfile>> GetAllUsersAsync();
    Task AddUserAsync(UserProfile user);
    void UpdateUser(UserProfile user);
    void DeleteUser(UserProfile user);
    Task SaveChangesAsync();
}
