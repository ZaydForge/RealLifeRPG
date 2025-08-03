using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using TaskManagement.Application.Security;

namespace TaskManagement.Application.Services.Impl;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int? UserId 
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null)
            {
                Console.WriteLine("CurrentUserService: HttpContext.User is null");
                return null;
            }

            if (user.Identity?.IsAuthenticated != true)
            {
                Console.WriteLine($"CurrentUserService: User is not authenticated. IsAuthenticated: {user.Identity?.IsAuthenticated}");
                return null;
            }

            Console.WriteLine($"CurrentUserService: User is authenticated. Claims count: {user.Claims.Count()}");
            foreach (var claim in user.Claims)
            {
                Console.WriteLine($"CurrentUserService: Claim - Type: {claim.Type}, Value: {claim.Value}");
            }

            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Console.WriteLine($"CurrentUserService: NameIdentifier claim value: {userIdClaim}");
            
            if (string.IsNullOrEmpty(userIdClaim))
            {
                Console.WriteLine("CurrentUserService: NameIdentifier claim is null or empty");
                return null;
            }

            if (int.TryParse(userIdClaim, out var userId))
            {
                Console.WriteLine($"CurrentUserService: Successfully parsed userId: {userId}");
                return userId;
            }
            else
            {
                Console.WriteLine($"CurrentUserService: Failed to parse userId from claim: {userIdClaim}");
                return null;
            }
        }
    }

    public IUser? User
    {
        get
        {
            if (_httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated != true)
                return null;

            var userIdClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdClaim, out var userId))
                return null;

            var fullName = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Name)?.Value ?? "";
            var permissions = _httpContextAccessor.HttpContext.User.Claims
                .Where(x => x.Type == "permission")
                .Select(x => x.Value);
            var isAdmin = bool.TryParse(_httpContextAccessor.HttpContext.User.FindFirst("isAdmin")?.Value, out var admin) && admin;
            var isVerified = bool.TryParse(_httpContextAccessor.HttpContext.User.FindFirst("isVerified")?.Value, out var verified) && verified;

            return new UserAuthModel
            {
                Id = userId,
                FullName = fullName,
                Permissions = permissions,
                IsAdmin = isAdmin,
                IsVerified = isVerified
            };
        }
    }
}