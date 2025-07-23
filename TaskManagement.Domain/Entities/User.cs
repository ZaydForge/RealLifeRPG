namespace TaskManagement.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string Fullname { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public DateTime? DateOfBirth { get; set; }
    public string PasswordHash { get; set; } = null!;
    public string Salt { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsVerified { get; set; } = false;

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
