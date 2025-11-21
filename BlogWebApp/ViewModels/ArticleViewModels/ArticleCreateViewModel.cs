using BlogWebApp.Models;
using System.ComponentModel.DataAnnotations;

namespace BlogWebApp.ViewModels.ArticleViewModels
{
    public class ArticleCreateViewModel
    {
        [Required(ErrorMessage = "Заголовок обязателен.")]
        [MaxLength(300, ErrorMessage = "Длина заголовка не должна превышать 300 символов.")]
        public string Title { get; set; } = null!;

        public string? Image { get; set; }

        [Required(ErrorMessage = "Текст поста обязателен.")]
        public string Content { get; set; } = null!;

        [Required(ErrorMessage = "Жанр обязателен.")]
        public Guid GenreId { get; set; }

        public List<GenreViewModel> Genres { get; set; } = new();
    }
}
