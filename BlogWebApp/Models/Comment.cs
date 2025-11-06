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
        public string Content { get; set; } = null!;

        [Required]
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        [Required]
        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;

        [Required]
        public Guid ArticleId { get; set; }
        public Article Article { get; set; } = null!;

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
