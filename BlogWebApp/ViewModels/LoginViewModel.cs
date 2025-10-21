using System.ComponentModel.DataAnnotations;

namespace BlogWebApp.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [MaxLength(200)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
