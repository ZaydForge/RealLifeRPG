using TaskManagement.Domain.Enums;

namespace TaskManagement.Entities;

public class TaskLog
{
    public int Id { get; set; }

    public int TaskId { get; set; }

    public string TaskTitle { get; set; } = string.Empty;

    public string TaskDescription { get; set; } = string.Empty;

    public int UserId { get; set; }
    public UserProfile User { get; set; }

    public Category Category { get; set; }

    public DateTime CompletedAt { get; set; } = DateTime.UtcNow;

    public int EXP_Gained { get; set; }
}
