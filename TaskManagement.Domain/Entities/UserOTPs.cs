namespace TaskManagement.Domain.Entities;

public class UserOTPs
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Code { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiredAt { get; set; }
    public bool IsUsed { get; set; } = false;
    public int AttemptCount { get; set; } = 0;
    public DateTime? LastAttemptAt { get; set; }
    public string Purpose { get; set; } = null!; // "EmailVerification", "PasswordReset", etc.

    public User User { get; set; } = null!;
}
