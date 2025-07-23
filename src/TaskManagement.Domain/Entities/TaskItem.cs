using TaskManagement.Domain.Enums;

namespace TaskManagement.Entities;

public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public int EXPValue { get; set; } = 10;

    public Category Category { get; set; }

    public int UserId { get; set; }
    public AppUser User { get; set; }
}
