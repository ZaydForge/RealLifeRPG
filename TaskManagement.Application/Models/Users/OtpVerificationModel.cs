using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Application.Models.Users;

public class OtpVerificationModel
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = null!;
    
    [Required(ErrorMessage = "OTP code is required")]
    [StringLength(6, MinimumLength = 6, ErrorMessage = "OTP code must be exactly 6 digits")]
    [RegularExpression(@"^\d{6}$", ErrorMessage = "OTP code must contain only digits")]
    public string Code { get; set; } = null!;
}
