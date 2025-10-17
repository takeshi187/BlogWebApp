using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogWebApp.Models
{
    public class Comment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CommentId { get; private set; }

        [Required]
        [MaxLength(500)]
        public string Content { get; set; }

        [Required]
        public DateOnly CreatedAt { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

        [Required]
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
        public int UserId { get; set; }

        [Required]
        [ForeignKey(nameof(ArticleId))]
        public Article Article { get; set; }
        public int ArticleId { get; set; }

        public Comment(string content, int userId, int articleId)
        {
            Content = content;
            UserId = userId;
            ArticleId = articleId;
        }

        // for tests
        public Comment(int commentId, string content, int userId, int articleId)
        {
            CommentId = commentId;
            Content = content;
            UserId = userId;
            ArticleId = articleId;
        }

        public Comment() { }
    }
}
