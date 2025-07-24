using System.Buffers.Text;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Entities;

public class UserProfile
{
    public int Id {get; set;}

    public int UserId { get; set; }
    public User User { get; set; } = null!;


    public string Username { get; set; } = null!;
    public string? Bio { get; set; } = string.Empty;
    public string? ProfilePictureUrl { get; set; } = string.Empty;

    public int CurrentStreak { get; set; } = 0;
    public int LongestStreak { get; set; } = 0;


    public string CurrentTitle { get; set; } = "The Beginning"; 
    public ICollection<UserTitle> Titles { get; set; } = new List<UserTitle>(); 
    public ICollection<UserAchievement> Achievements { get; set; } = new List<UserAchievement>(); 

    public int MainLevel { get; set; } = 1;
    public int TotalExp { get; set; } = 0;
    public DateTime LastLevelUp { get; set; } = DateTime.UtcNow;


    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    public ICollection<CategoryLevel> CategoryLevels { get; set; } = new List<CategoryLevel>();
    public ICollection<TaskLog> TaskLogs { get; set; } = new List<TaskLog>();

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}
