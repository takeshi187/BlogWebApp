using BlogWebApp.Models;
using System.ComponentModel.DataAnnotations;

namespace BlogWebApp.ViewModels
{
    public class ArticleViewModel
    {
        public Guid ArticleViewModelId { get; set; } = Guid.NewGuid();
        [Required(ErrorMessage = "Заголовок обязателен.")]
        [MaxLength(300, ErrorMessage = "Длина заголовка не должна превышать 300 символов.")]
        public string Title { get; set; } = null!;

        public string? Image { get; set; }

        [Required(ErrorMessage = "Текст поста обязателен.")]
        [MinLength(10, ErrorMessage = "Статья должна содержать минимум 10 символов.")]
        public string Content { get; set; } = null!;

        [Required(ErrorMessage = "Жанр обязателен.")]
        public Guid GenreId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
