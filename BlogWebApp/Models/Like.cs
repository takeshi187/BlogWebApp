using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogWebApp.Models
{
    public class Like
    {
        [Key]
        public Guid LikeId { get; private set; } = Guid.NewGuid();

        [Required]
        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;

        [Required]
        public Guid ArticleId { get; set; }
        public Article Article { get; set; } = null!;

        [Required]
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

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

        private Like() { }
    }
}
