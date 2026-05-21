using System.ComponentModel.DataAnnotations;

namespace StudyPlanner.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Имя пользователя обязательно")]
        [StringLength(100, MinimumLength = 3)]
        public string Username { get; set; }

        [Required(ErrorMessage = "Email обязателен")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Пароль обязателен")]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; }
    }
}