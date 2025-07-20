using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Helpers.GenerateJwt;

public interface IJwtTokenHandler
{
    string GenerateAccessToken(User user, string sessionToken);
    string GenerateRefreshToken();
}
