using TaskManagement.Application.Models;
using TaskManagement.Application.Models.Users;

namespace TaskManagement.Application.Services;

public interface IUserService
{
    Task<ApiResult<string>> RegisterAsync(RegisterUserModel model);
    Task<ApiResult<LoginResponseModel>> LoginAsync(LoginUserModel model);
    Task<ApiResult<string>> VerifyOtpAsync(OtpVerificationModel model);
    Task<ApiResult<UserAuthResponseModel>> GetUserAuth();
    Task<ApiResult<string>> ForgotPasswordAsync(string email);
    Task<ApiResult<string>> ResetPasswordAsync(string email, string code, string newPassword);
    Task<ApiResult<string>> DeleteUserAsync(string email);
    Task<ApiResult<string>> ResendOtpAsync(string email);

}
