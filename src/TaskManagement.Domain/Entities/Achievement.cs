using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Domain.Entities
{
    public class Achievement
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
        
        public string UnlockRule { get; set; } = string.Empty;
    }
}
