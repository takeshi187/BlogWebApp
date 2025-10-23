using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogWebApp.Models
{
    public class Comment
    {
        [Key]
        public Guid CommentId { get; private set; } = Guid.NewGuid();

        [Required]
        [MaxLength(500)]
        public string Content { get; set; }

        [Required]
        public DateOnly CreatedAt { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

        public DateOnly? UpdatedAt { get; set; }

        [Required]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        [Required]
        public Guid ArticleId { get; set; }
        public Article Article { get; set; }

        public Comment(string content, string userId, Guid articleId)
        {
            Content = content;
            UserId = userId;
            ArticleId = articleId;
        }

        // for tests
        public Comment(Guid commentId, string content, string userId, Guid articleId)
        {
            CommentId = commentId;
            Content = content;
            UserId = userId;
            ArticleId = articleId;
        }

        public Comment() { }
    }
}
