using TaskManagement.Domain.Enums;

namespace TaskManagement.Dtos;

public class CreateTaskDto
{
    public string? Title { get; set; }

    public string? Description { get; set; }
    
    public int EXPValue { get; set; }

    public Category Category { get; set; }

    public int UserId { get; set; }

}
