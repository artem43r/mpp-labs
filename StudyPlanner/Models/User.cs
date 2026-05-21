using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace StudyPlanner.Models
{
    public class User : IdentityUser<int>
    {
        public string? Avatar { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string? Settings { get; set; }

        // Навигационные свойства
        public ICollection<Subject> Subjects { get; set; } = new List<Subject>();
        public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
        public ICollection<Tag> Tags { get; set; } = new List<Tag>();
    }
}