using TaskManagement.Domain.Enums;

namespace TaskManagement.Entities;

public class Category
{
    public int Id { get; set; }

    public CategoryName CategoryName { get; set; }

}
