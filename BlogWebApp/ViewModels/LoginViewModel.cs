using System.ComponentModel.DataAnnotations;

namespace BlogWebApp.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email обязателен.")]
        [EmailAddress(ErrorMessage = "Некорректный email.")]
        [MaxLength(255, ErrorMessage = "Длина email не должна превышать 255 символов.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Пароль обязателен.")]
        [DataType(DataType.Password, ErrorMessage = "Некорректный пароль.")]
        [MinLength(8, ErrorMessage = "Длина пароля должна быть не менее '8' символов.")]
        public string Password { get; set; } = null!;

        [Display(Name = "Запомнить меня")]
        public bool RememberMe { get; set; }
    }
}
