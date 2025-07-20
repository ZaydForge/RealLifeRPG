using TaskManagement.Application.Models;
using TaskManagement.Application.Models.Users;

namespace TaskManagement.Application.Services;

public interface IUserService
{
    Task<ApiResult<string>> RegisterAsync(string fullname, string email, string password, bool isAdminSite);
    Task<ApiResult<LoginResponseModel>> LoginAsync(LoginUserModel model);
    Task<ApiResult<string>> VerifyOtpAsync(OtpVerificationModel model);
    Task<ApiResult<UserAuthResponseModel>> GetUserAuth();
}
