using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace StudyPlanner.Models
{
    public class Tag
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Название тега обязательно")]
        [StringLength(50, ErrorMessage = "Название тега не может превышать 50 символов")]
        public string Name { get; set; }

        // FK
        public int OwnerId { get; set; }

        // Навигация
        public User Owner { get; set; }

        public ICollection<AssignmentTag> AssignmentTags { get; set; } = new List<AssignmentTag>();
    }
}
