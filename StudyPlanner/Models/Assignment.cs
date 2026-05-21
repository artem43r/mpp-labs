using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace StudyPlanner.Models
{
    public class Assignment
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Заголовок обязателен")]
        [StringLength(200, ErrorMessage = "Заголовок не может превышать 200 символов")]
        public string Title { get; set; }

        public string? Description { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [DataType(DataType.DateTime)]
        [CustomValidation(typeof(Assignment), nameof(ValidateDeadline))]
        public DateTime? Deadline { get; set; }

        [Required]
        public string Status { get; set; } = "New";

        [Required]
        public string Priority { get; set; } = "Medium";

        // ДОБАВКА ИЗ ТВОЕЙ ЛАБЫ 1
        public string Recurrence { get; set; } = "None";

        // FK
        public int? SubjectId { get; set; }
        public int UserId { get; set; }

        // Навигация
        [ForeignKey("SubjectId")]
        public Subject? Subject { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }

        public ICollection<AssignmentTag> AssignmentTags { get; set; } = new List<AssignmentTag>();

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

        // Валидация дедлайна
        public static ValidationResult? ValidateDeadline(DateTime? deadline, ValidationContext context)
        {
            if (deadline.HasValue && deadline.Value < DateTime.UtcNow.Date)
            {
                return new ValidationResult("Дедлайн не может быть в прошлом");
            }
            return ValidationResult.Success;
        }
    }
}
