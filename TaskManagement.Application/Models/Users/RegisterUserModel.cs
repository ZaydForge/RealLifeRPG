namespace TaskManagement.Application.Models.Users
{
    public class RegisterUserModel
    {
        public string Fullname { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Password { get; set; }
        public bool isAdminSite { get; set; }
    }
}
