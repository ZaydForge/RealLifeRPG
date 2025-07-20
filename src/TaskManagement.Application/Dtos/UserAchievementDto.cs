using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Application.Dtos
{
    public class UserAchievementDto
    {
        public string UserName { get; set; }

        public string Name { get; set; }

        public DateTime UnlockedAt { get; set; } = DateTime.UtcNow;
    }
}
