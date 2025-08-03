using TaskManagement.Entities;

namespace TaskManagement.Domain.Entities
{
    public class UserCategory
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int CategoryId { get; set; }

        public virtual UserProfile User { get; set; }

        public virtual Category Category { get; set; }

        public int Level { get; set; } = 1;
        public int CurrentEXP { get; set; } = 0;
        public int EXPToNextLevel { get; set; } = 100;
        public int NeededEXP { get; set; } = 100;

        public DateTime LastLevelUp { get; set; } = DateTime.UtcNow;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;
    }
}
