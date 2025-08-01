﻿namespace TaskManagement.Application.Models.Users;

public class LoginResponseModel
{
    public string Email { get; set; } = null!;
    public string Fullname { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public DateTime? DateOfBirth { get; set; }

    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;

    public List<string> Roles { get; set; } = new();
    public List<string> Permissions { get; set; } = new();
}
