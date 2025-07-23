namespace TaskManagement.Dtos;

public class TaskDto
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedDate { get; set; }
    public int EXPValue { get; set; }
    public string? Category { get; set; }
}
