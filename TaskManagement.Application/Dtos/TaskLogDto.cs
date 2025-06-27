namespace TaskManagement.Dtos;

public class TaskLogDto
{
    public int Id { get; set; }

    public string? TaskTitle { get; set; }

    public string? Category {  get; set; }

    public DateTime CompletedAt { get; set; }

    public int EXP_Gained { get; set; }
}
