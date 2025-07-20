﻿namespace TaskManagement.Domain.Entities;

public class RolePermission
{
    public int Id { get; set; }
    public int RoleId { get; set; }
    public int PermissionId { get; set; }
    public Role Role { get; set; } = null!;
    public Permission Permission { get; set; } = null!;

    public ICollection<User> Users { get; set; } = new List<User>();
}
