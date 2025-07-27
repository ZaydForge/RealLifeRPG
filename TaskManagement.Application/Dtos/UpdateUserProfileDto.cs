using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Application.Dtos
{
    public class UpdateUserProfileDto
    {
        public string? Bio { get; set; }
        public IFormFile? ProfilePicture { get; set; }
    }
}
