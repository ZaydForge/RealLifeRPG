using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Application.Dtos
{
    public class UserTitleDto
    {
        public string UserName { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public bool IsActive { get; set; } = false;

        public DateTime UnlockedAt { get; set; } = DateTime.UtcNow;
    }
}
