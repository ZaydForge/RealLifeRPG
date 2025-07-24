using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Entities;

namespace TaskManagement.Domain.Entities
{
    public class UserAchievement
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public int AchievementId { get; set; }

        public DateTime UnlockedAt { get; set; } = DateTime.UtcNow;

        public UserProfile User { get; set; }
        public Achievement Achievement { get; set; }
    }
}
