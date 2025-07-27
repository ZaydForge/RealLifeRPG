using TaskManagement.Domain.Enums;

namespace TaskManagement.Entities;

public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public int EXPValue { get; set; } = 10;

    public Category Category { get; set; }

    public int UserId { get; set; }
    public UserProfile User { get; set; } = null!;

    public ExpirationType ExpirationType { get; set; } = ExpirationType.Urgent;

    public DateTime ExpiresAt { get; set; }

    public Domain.Enums.TaskStatus Status { get; set; } = Domain.Enums.TaskStatus.Active;

    public bool IsSaved { get; set; } = false;
}
