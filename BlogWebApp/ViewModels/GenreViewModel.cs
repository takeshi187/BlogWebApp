using System.ComponentModel.DataAnnotations;

namespace BlogWebApp.ViewModels
{
    public class GenreViewModel
    {
        public Guid GenreId { get; set; }
        [Required(ErrorMessage = "Имя жанра должно быть заполнено.")]
        [MaxLength(255, ErrorMessage = "Имя жанра не должно превышать 255 символов.")]
        public string GenreName { get; set; } = null!;
    }
}
