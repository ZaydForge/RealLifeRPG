using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Helpers.PasswordHashers;
using TaskManagement.Application.Models.Users;
using TaskManagement.Application.Models;
using TaskManagement.Domain.Entities;
using TaskManagement.Persistence;
using TaskManagement.Application.Helpers.GenerateJwt;
using TaskManagement.DataAccess;

namespace TaskManagement.Application.Services.Impl;

public class UserService : IUserService
{
    private readonly DataContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IOtpService _otpService;
    private readonly IEmailService _emailService;
    private readonly IJwtTokenHandler _jwtTokenHandler;
    private readonly IAuthService _authService;

    public UserService(
        DataContext context,
        IPasswordHasher passwordHasher,
        IOtpService otpService,
        IEmailService emailService,
        IJwtTokenHandler jwtTokenHandler,
        IAuthService authService)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _otpService = otpService;
        _emailService = emailService;
        _jwtTokenHandler = jwtTokenHandler;
        _authService = authService;
    }

    public async Task<ApiResult<string>> RegisterAsync(RegisterUserModel model)
    {
        var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
        if (existingUser != null)
            return ApiResult<string>.Failure(new[] { "Email already exists" }, "Email already exists");

        var salt = Guid.NewGuid().ToString();
        var hash = _passwordHasher.Encrypt(model.Password, salt);

        var user = new User
        {
            Fullname = model.Fullname,
            Email = model.Email,
            PhoneNumber = model.PhoneNumber,
            Username = model.Username,
            DateOfBirth = model.DateOfBirth,
            PasswordHash = hash,
            Salt = salt,
            CreatedAt = DateTime.UtcNow,
            IsVerified = false // Yangi foydalanuvchilar odatda tasdiqlanmagan holda boshlanadi
        };

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        // --- Rolni isAdminSite ga qarab belgilash ---
        string roleName = model.isAdminSite ? "Admin" : "User";
        var defaultRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);

        if (defaultRole == null)
        {
            // Agar kerakli rol topilmasa, xato qaytaramiz
            return ApiResult<string>.Failure(new[] { $"The role '{roleName}' was not found in the system. Please contact the administrator." }, $"The role '{roleName}' was not found in the system. Please contact the administrator.");
        }

        _context.UserRoles.Add(new UserRole
        {
            UserId = user.Id,
            RoleId = defaultRole.Id
        });
        await _context.SaveChangesAsync();
        // --- Rolni belgilash qismi tugadi ---

        var otp = await _otpService.GenerateAndSaveOtpAsync(user.Id);
        await _emailService.SendOtpAsync(model.Email, otp);

        return ApiResult<string>.Success("You have registered. Please verify via email", "You have registered. Please verify via email");
    }

    public async Task<ApiResult<LoginResponseModel>> LoginAsync(LoginUserModel model)
    {
        var user = await _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Email == model.Email);

        if (user is null)
            return ApiResult<LoginResponseModel>.Failure(new[] { "User not found" }, "User not found");

        if (!_passwordHasher.Verify(user.PasswordHash, model.Password, user.Salt))
            return ApiResult<LoginResponseModel>.Failure(new[] { "Incorrect password" }, "Incorrect password");

        if (!user.IsVerified)
            return ApiResult<LoginResponseModel>.Failure(new[] { "Email is not verified" }, "Email is not verified");

        var accessToken = _jwtTokenHandler.GenerateAccessToken(user, Guid.NewGuid().ToString());
        var refreshToken = _jwtTokenHandler.GenerateRefreshToken();

        return ApiResult<LoginResponseModel>.Success(new LoginResponseModel
        {
            Email = user.Email,
            Fullname = user.Fullname,
            Username = user.Username ?? "",
            PhoneNumber = user.PhoneNumber ?? "",
            DateOfBirth = user.DateOfBirth,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList(),
            Permissions = user.UserRoles
                 .SelectMany(ur => ur.Role.RolePermissions)
                 .Select(p => p.Permission.ShortName)
                 .Distinct()
                 .ToList()
        }, "Successfully logged in");
    }

    public async Task<ApiResult<string>> VerifyOtpAsync(OtpVerificationModel model)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
        if (user is null)
            return ApiResult<string>.Failure(new[] { "User not found" }, "User not found");   

        var otp = await _otpService.GetLatestOtpAsync(user.Id, model.Code);
        if (otp is null)
            return ApiResult<string>.Failure(new[] { "The code is incorrect or has expired" }, "The code is incorrect or has expired");

        user.IsVerified = true;
        await _context.SaveChangesAsync();

        return ApiResult<string>.Success("Successfully verified", "Successfully verified");
    }

    public async Task<ApiResult<UserAuthResponseModel>> GetUserAuth()
    {
        if (_authService.User == null)
        {
            return ApiResult<UserAuthResponseModel>.Failure(new List<string> { "User not found" }, "User not found");
        }

        UserAuthResponseModel userPermissions = new UserAuthResponseModel
        {
            Id = _authService.User.Id,
            FullName = _authService.User.FullName,
            Permissions = _authService.User.Permissions
        };

        return ApiResult<UserAuthResponseModel>.Success(userPermissions, "You have successfully registered!");
    }

    public async Task<ApiResult<string>> ForgotPasswordAsync(string email)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user is null)
            return ApiResult<string>.Failure(["User not found"], "User not found");

        var otp = await _otpService.GenerateAndSaveOtpAsync(user.Id);
        await _emailService.SendOtpAsync(email, otp);

        return ApiResult<string>.Success("OTP has been sent to your email.", "OTP has been sent to your email.");
    }

    public async Task<ApiResult<string>> ResetPasswordAsync(string email, string code, string newPassword)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user is null)
            return ApiResult<string>.Failure(["User not found"], "User not found");

        var otp = await _otpService.GetLatestOtpAsync(user.Id, code);
        if (otp == null || otp.ExpiredAt < DateTime.UtcNow)
            return ApiResult<string>.Failure(["Invalid or expired code"], "Invalid or expired code");

        var salt = Guid.NewGuid().ToString();
        var hashedPassword = _passwordHasher.Encrypt(newPassword, salt);

        user.PasswordHash = hashedPassword;
        user.Salt = salt;
        await _context.SaveChangesAsync();

        return ApiResult<string>.Success("Password has been reset successfully", "Password has been reset successfully");
    }

    public async Task<ApiResult<string>> ResendOtpAsync(string email)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user is null)
            return ApiResult<string>.Failure(new[] { "User not found" }, "User not found");

        if (user.IsVerified)
            return ApiResult<string>.Failure(new[] { "This email is already verified" }, "This email is already verified");

        var otp = await _otpService.GenerateAndSaveOtpAsync(user.Id); // generate new OTP
        await _emailService.SendOtpAsync(email, otp); // send it again

        return ApiResult<string>.Success("OTP has been resent to your email", "OTP has been resent to your email");
    }


    public async Task<ApiResult<string>> DeleteUserAsync(string email)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user is null)
            return ApiResult<string>.Failure(new[] { "User not found" }, "User not found");

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return ApiResult<string>.Success("User deleted successfully", "User deleted successfully");
    }

}
