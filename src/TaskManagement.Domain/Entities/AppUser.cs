using System.Buffers.Text;

namespace TaskManagement.Entities;

public class AppUser
{
    public int Id {get; set;}

    public string Username {get; set;}

    public string Email { get; set;}

    public string PasswordHash { get; set;}

    public int MainLevel { get; set; } = 1;

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public ICollection<TaskItem> Tasks { get; set; }
    public ICollection<CategoryLevel> CategoryLevels { get; set; }
    public ICollection<TaskLog> TaskLogs { get; set; }
}
