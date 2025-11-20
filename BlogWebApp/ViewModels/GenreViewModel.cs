using System.ComponentModel.DataAnnotations;

namespace BlogWebApp.ViewModels
{
    public class GenreViewModel
    {
        public Guid GenreId { get; set; }
        [Required(ErrorMessage = "Имя жанра должно быть заполнено.")]
        [MaxLength(150, ErrorMessage = "Имя жанра не должно превышать 150 символов.")]
        public string GenreName { get; set; } = null!;
    }
}
