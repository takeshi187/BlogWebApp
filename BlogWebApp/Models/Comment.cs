using System.ComponentModel.DataAnnotations;

namespace BlogWebApp.Models
{
    public class Comment
    {
        [Key]
        public Guid CommentId { get; private set; }

        [Required]
        [MaxLength(500)]
        public string Content { get; private set; } = null!;

        [Required]
        public DateTime CreatedAt { get; private set; }

        [Required]
        public string UserId { get; private set; }
        public ApplicationUser User { get; private set; } = null!;

        [Required]
        public Guid ArticleId { get; private set; }
        public Article Article { get; private set; } = null!;

        public Comment(string content, string userId, Guid articleId)
        {
            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("Content cannot be empty.", nameof(content));
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("UserId cannot be empty.", nameof(userId));
            if (articleId == Guid.Empty)
                throw new ArgumentException("ArticleId cannot be empty.", nameof(articleId));

            CommentId = Guid.NewGuid();
            Content = content;
            UserId = userId;
            ArticleId = articleId;
            CreatedAt = DateTime.UtcNow;
        }

        public Comment(string content, ApplicationUser user, Guid articleId)
        {
            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("Content cannot be empty.", nameof(content));
            if (user == null)
                throw new ArgumentNullException("User cannot be empty.", nameof(user));
            if (articleId == Guid.Empty)
                throw new ArgumentException("ArticleId cannot be empty.", nameof(articleId));

            CommentId = Guid.NewGuid();
            Content = content;
            User = user;
            ArticleId = articleId;
            CreatedAt = DateTime.UtcNow;
        }

        protected Comment() { }
    }
}
