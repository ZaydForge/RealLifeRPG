using TaskManagement.Entities;

namespace TaskManagement.Domain.Entities
{
    public class UserTitle
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public int TitleId { get; set; }

        public bool IsActive { get; set; } = false;

        public DateTime UnlockedAt { get; set; } = DateTime.UtcNow;

        public UserProfile User { get; set; }
        public Title Title { get; set; }
    }
}
