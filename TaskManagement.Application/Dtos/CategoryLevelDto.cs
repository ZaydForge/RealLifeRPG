using TaskManagement.Entities;

namespace TaskManagement.Dtos;

public class CategoryLevelDto
{
    public string? Category { get; set; }
    public int Level { get; set; }
    public int CurrentEXP { get; set; }
    public int EXPToNextLevel { get; set; }
}
