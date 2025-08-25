using Microsoft.EntityFrameworkCore;
using TaskManagement.DataAccess;
using TaskManagement.Domain.Entities;
using TaskManagement.Persistence;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging;

namespace TaskManagement.Application.Services.Impl;

public class OtpService : IOtpService
{
    private readonly DataContext _context;
    private readonly IEmailService _emailService;
    private readonly ILogger<OtpService> _logger;
    
    private const int OtpLength = 6;
    private const int OtpExpiryMinutes = 10;
    private const int MaxOtpAttempts = 5;
    private const int RateLimitMinutes = 5;
    private const int MaxOtpRequestsPerHour = 3;

    public OtpService(DataContext context, IEmailService emailService, ILogger<OtpService> logger)
    {
        _context = context;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<string> GenerateAndSaveOtpAsync(int userId, string purpose = "EmailVerification")
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            _logger.LogWarning("OTP generation attempted for non-existent user {UserId}", userId);
            throw new Exception("User not found");
        }

        if (!await CanRequestOtpAsync(userId, purpose))
        {
            _logger.LogWarning("OTP rate limit exceeded for user {UserId} with purpose {Purpose}", userId, purpose);
            throw new Exception("Too many OTP requests. Please wait before requesting a new code.");
        }

        await InvalidateOtpsAsync(userId, purpose);

        var otpCode = GenerateSecureOtp();

        var otp = new UserOTPs
        {
            UserId = userId,
            Code = otpCode,
            CreatedAt = DateTime.UtcNow,
            ExpiredAt = DateTime.UtcNow.AddMinutes(OtpExpiryMinutes),
            Purpose = purpose,
            IsUsed = false,
            AttemptCount = 0
        };

        await _context.UserOTPs.AddAsync(otp);
        await _context.SaveChangesAsync();

        await _emailService.SendOtpAsync(user.Email, otpCode);
        
        _logger.LogInformation("OTP generated successfully for user {UserId} with purpose {Purpose}", userId, purpose);
        return otpCode;
    }

    public async Task<(bool IsValid, string? ErrorMessage)> VerifyOtpAsync(int userId, string code, string purpose = "EmailVerification")
    {
        if (string.IsNullOrWhiteSpace(code) || code.Length != OtpLength || !code.All(char.IsDigit))
        {
            _logger.LogWarning("Invalid OTP format provided for user {UserId}", userId);
            return (false, "Invalid OTP format");
        }

        var otp = await _context.UserOTPs
            .Where(o => o.UserId == userId && o.Purpose == purpose && !o.IsUsed)
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefaultAsync();

        if (otp == null)
        {
            _logger.LogWarning("No valid OTP found for user {UserId} with purpose {Purpose}", userId, purpose);
            return (false, "No valid OTP found");
        }

        otp.AttemptCount++;
        otp.LastAttemptAt = DateTime.UtcNow;

        if (otp.AttemptCount > MaxOtpAttempts)
        {
            otp.IsUsed = true;
            await _context.SaveChangesAsync();
            _logger.LogWarning("OTP attempt limit exceeded for user {UserId}", userId);
            return (false, "Too many attempts. Please request a new code.");
        }

        if (otp.ExpiredAt < DateTime.UtcNow)
        {
            otp.IsUsed = true;
            await _context.SaveChangesAsync();
            _logger.LogWarning("Expired OTP used for user {UserId}", userId);
            return (false, "OTP has expired");
        }

        bool isValidCode = ConstantTimeEquals(otp.Code, code);

        if (!isValidCode)
        {
            await _context.SaveChangesAsync();
            _logger.LogWarning("Invalid OTP code provided for user {UserId}, attempt {AttemptCount}", userId, otp.AttemptCount);
            return (false, "Invalid OTP code");
        }

        otp.IsUsed = true;
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("OTP verified successfully for user {UserId} with purpose {Purpose}", userId, purpose);
        return (true, null);
    }

    public async Task<bool> CanRequestOtpAsync(int userId, string purpose = "EmailVerification")
    {
        var oneHourAgo = DateTime.UtcNow.AddHours(-1);
        var recentOtpCount = await _context.UserOTPs
            .CountAsync(o => o.UserId == userId && 
                           o.Purpose == purpose && 
                           o.CreatedAt > oneHourAgo);

        if (recentOtpCount >= MaxOtpRequestsPerHour)
        {
            return false;
        }

        var rateLimitTime = DateTime.UtcNow.AddMinutes(-RateLimitMinutes);
        var recentOtp = await _context.UserOTPs
            .Where(o => o.UserId == userId && 
                       o.Purpose == purpose && 
                       o.CreatedAt > rateLimitTime)
            .FirstOrDefaultAsync();

        return recentOtp == null;
    }

    public async Task InvalidateOtpsAsync(int userId, string purpose)
    {
        var activeOtps = await _context.UserOTPs
            .Where(o => o.UserId == userId && o.Purpose == purpose && !o.IsUsed)
            .ToListAsync();

        foreach (var otp in activeOtps)
        {
            otp.IsUsed = true;
        }

        if (activeOtps.Any())
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation("Invalidated {Count} active OTPs for user {UserId} with purpose {Purpose}", 
                activeOtps.Count, userId, purpose);
        }
    }

    private static string GenerateSecureOtp()
    {
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[4];
        rng.GetBytes(bytes);
        var randomNumber = BitConverter.ToUInt32(bytes, 0);
        var otp = (randomNumber % 900000) + 100000;
        return otp.ToString();
    }

    private static bool ConstantTimeEquals(string a, string b)
    {
        if (a.Length != b.Length)
            return false;

        var result = 0;
        for (int i = 0; i < a.Length; i++)
        {
            result |= a[i] ^ b[i];
        }
        return result == 0;
    }
}
