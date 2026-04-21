using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace StudyPlanner.Models
{

    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Имя пользователя обязательно")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Имя должно содержать от 3 до 100 символов")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Email обязателен")]
        [EmailAddress(ErrorMessage = "Некорректный формат email")]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

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
