namespace TaskManagement.Application.Services
{
    public interface IUserProfileService
    {
        Task CreateUserProfileAsync(int userId);
    }
}
