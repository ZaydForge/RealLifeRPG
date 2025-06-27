using System.ComponentModel.DataAnnotations;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Entities;

public class Archive
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public DateTime CreatedDate { get; set; }

    public int EXPValue { get; set; }

    public Category Category { get; set; }
}
