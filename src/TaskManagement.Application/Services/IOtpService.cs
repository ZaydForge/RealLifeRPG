using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Services;

public interface IOtpService
{
    Task<string> GenerateAndSaveOtpAsync(int userId);
    Task<UserOTPs?> GetLatestOtpAsync(int userId, string code);
}
