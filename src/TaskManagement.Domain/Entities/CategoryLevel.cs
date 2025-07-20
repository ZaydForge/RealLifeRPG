using TaskManagement.Domain.Enums;

namespace TaskManagement.Entities;

public class CategoryLevel
{
    public int Id { get; set; }

    public Category Category { get; set; }
    public int Level { get; set; } = 1;
    public int CurrentEXP { get; set; } = 0;
    public int EXPToNextLevel { get; set; } = 100;
    public int NeededEXP { get; set; } = 100;

    public int UserId { get; set; }
    public AppUser User { get; set; }
}
