using System.ComponentModel.DataAnnotations;

namespace BlogWebApp.Models
{
    public class Like
    {
        [Key]
        public Guid LikeId { get; private set; }

        [Required]
        public string UserId { get; private set; } = null!;
        public ApplicationUser User { get; private set; } = null!;

        [Required]
        public Guid ArticleId { get; private set; }
        public Article Article { get; private set; } = null!;

        [Required]
        public DateTime CreatedAt { get; private set; }

        public Like(string userId, Guid articleId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("UserId cannot be empty.", nameof(userId));

            if (articleId == Guid.Empty)
                throw new ArgumentException("ArticleId cannot be empty.", nameof(articleId));

            LikeId = Guid.NewGuid();
            UserId = userId;
            ArticleId = articleId;
            CreatedAt = DateTime.UtcNow;
        }

        protected Like() { }
    }
}
