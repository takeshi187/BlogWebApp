using BlogWebApp.Validators;
using System.ComponentModel.DataAnnotations;

namespace BlogWebApp.ViewModels
{
    public class ArticleViewModel
    {
        public Guid ArticleViewModelId { get; set; }

        [Required(ErrorMessage = "Заголовок обязателен.")]
        [MaxLength(300, ErrorMessage = "Длина заголовка не должна превышать '300' символов.")]
        public string Title { get; set; } = null!;

        public string? Image { get; set; }

        [Required(ErrorMessage = "Текст поста обязателен.")]
        public string Content { get; set; } = null!;

        [Required(ErrorMessage = "Жанр обязателен.")]
        [NotEmptyGuid(ErrorMessage = "Выберите жанр.")]
        public Guid GenreId { get; set; }
        public string? GenreName { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public int CommentsCount { get; set; }
        public int LikesCount { get; set; }

        public List<CommentViewModel> Comments { get; set; } = new();
        public List<GenreViewModel> Genres { get; set; } = new();

        public IFormFile? ImageFile { get; set; }

        public bool UserHasLiked { get; set; }
    }
}
