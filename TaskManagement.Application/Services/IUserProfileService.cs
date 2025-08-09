using TaskManagement.Entities;

namespace TaskManagement.Application.Services
{
    public interface IUserProfileService
    {
        Task<UserProfile> CreateUserProfileAsync(int userId);
    }
}
