using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogWebApp.Models
{
    public class Like
    {
        [Key]
        public Guid LikeId { get; private set; } = Guid.NewGuid();

        [Required]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        [Required]
        public Guid ArticleId { get; set; }
        public Article Article { get; set; }

        [Required]
        public DateOnly CreatedAt { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

        public Like(string userId, Guid articleId)
        {
            UserId = userId;
            ArticleId = articleId;
        }

        // for tests
        public Like(Guid likeId, string userId, Guid articleId)
        {
            LikeId = likeId;
            UserId = userId;
            ArticleId = articleId;
        }

        public Like() { }
    }
}
