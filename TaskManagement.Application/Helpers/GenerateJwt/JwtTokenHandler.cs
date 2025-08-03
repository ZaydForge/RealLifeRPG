using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TaskManagement.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TaskManagement.Entities;

namespace TaskManagement.Application.Helpers.GenerateJwt;

public class JwtTokenHandler : IJwtTokenHandler
{
    private readonly JwtOption _jwtOption;


    public JwtTokenHandler(IOptions<JwtOption> jwtOption)
    {
        _jwtOption = jwtOption.Value;
    }

    public string GenerateAccessToken(User user, string token)
    {
        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Fullname),
            new Claim("isVerified", user.IsVerified.ToString()),
            new Claim(CustomClaimNames.Token, token)
        };

        Console.WriteLine($"JWT Generation - Adding claims for user {user.Id}:");
        foreach (var claim in claims)
        {
            Console.WriteLine($"JWT Generation - Claim: Type='{claim.Type}', Value='{claim.Value}'");
        }

        if (user.UserRoles != null && user.UserRoles.Any())
        {
            var isAdmin = user.UserRoles.Any(ur => ur.Role.Name.Equals("Admin", StringComparison.OrdinalIgnoreCase));
            claims.Add(new Claim("isAdmin", isAdmin.ToString()));

            foreach (var userRole in user.UserRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole.Role.Name));

                if (userRole.Role.RolePermissions != null)
                {
                    foreach (var rolePermission in userRole.Role.RolePermissions)
                    {
                        claims.Add(new Claim("permission", rolePermission.Permission.ShortName));
                    }
                }
            }
        }
        else
        {
            claims.Add(new Claim("isAdmin", "false"));
        }


        Console.WriteLine($"JWT Generation - Final claims count: {claims.Count}");
        Console.WriteLine("JWT Generation - All final claims:");
        foreach (var claim in claims)
        {
            Console.WriteLine($"JWT Generation - Final Claim: Type='{claim.Type}', Value='{claim.Value}'");
        }

        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOption.SecretKey));

        var jwtToken = new JwtSecurityToken(
            issuer: _jwtOption.Issuer,
            audience: _jwtOption.Audience,
            expires: DateTime.UtcNow.AddSeconds(_jwtOption.ExpirationInSeconds),
            claims: claims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(jwtToken);
        Console.WriteLine($"JWT Generation - Generated token: {tokenString.Substring(0, Math.Min(50, tokenString.Length))}...");
        
        return tokenString;
    }

    public string GenerateRefreshToken()
    {
        byte[] bytes = new byte[64];

        using var randomGenerator =
            RandomNumberGenerator.Create();

        randomGenerator.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }
}
