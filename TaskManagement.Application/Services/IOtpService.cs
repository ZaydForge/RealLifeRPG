using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Services;

public interface IOtpService
{
    Task<string> GenerateAndSaveOtpAsync(int userId, string purpose = "EmailVerification");
    Task<(bool IsValid, string? ErrorMessage)> VerifyOtpAsync(int userId, string code, string purpose = "EmailVerification");
    Task<bool> CanRequestOtpAsync(int userId, string purpose = "EmailVerification");
    Task InvalidateOtpsAsync(int userId, string purpose);
}
