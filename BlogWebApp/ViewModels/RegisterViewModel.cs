using System.ComponentModel.DataAnnotations;

namespace BlogWebApp.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [MaxLength(200)]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [MaxLength(200)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароли не совпадают.")]
        public string ConfirmPassword { get; set; }
    }
}
