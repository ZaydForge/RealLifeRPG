using TaskManagement.Domain.Entities;
using TaskManagement.Entities;

namespace TaskManagement.Dtos;

public class UserProfileDto
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;
    public string? Bio { get; set; } = string.Empty;
    public string? ProfilePictureUrl { get; set; } = string.Empty;

    public int CurrentStreak { get; set; } = 0;
    public int LongestStreak { get; set; } = 0;


    public string CurrentTitle { get; set; } = "The Beginning";
    public int MainLevel { get; set; } = 1;
    public int TotalExp { get; set; } = 0;
    public DateTime LastLevelUp { get; set; } = DateTime.UtcNow;

}
