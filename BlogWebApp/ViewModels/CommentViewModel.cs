using System.ComponentModel.DataAnnotations;

namespace BlogWebApp.ViewModels
{
    public class CommentViewModel
    {
        public Guid CommentId { get; set; }

        [Required(ErrorMessage = "Комментарий не может быть пустым.")]
        [MaxLength(500, ErrorMessage = "Комментарий не должен превышать '500' символов.")]
        public string Content { get; set; } = null!;

        public Guid ArticleId { get; set; }

        public string UserName { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
    }
}
