using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogWebApp.Models
{
    public class Article
    {
        [Key]
        public Guid ArticleId { get; private set; } = Guid.NewGuid();

        [Required]
        [MaxLength(300)]
        public string Title { get; set; }

        public string? Image { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public Guid GenreId { get; set; }
        public Genre Genre { get; set; }

        [Required]
        public DateOnly CreatedAt { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

        public DateOnly? UpdatedAt { get; set; }

        public List<Comment> Comments { get; set; } = new();
        public List<Like> Likes { get; set; } = new();

        [NotMapped]
        public int LikesCount => Likes.Count;

        public Article(string title, string? image, string content, Guid genreId)
        {
            Title = title;
            Image = image;
            Content = content;
            GenreId = genreId;
        }

        // for tests
        public Article(Guid articleId, string title, string? image, string content, Guid genreId)
        {
            ArticleId = articleId;
            Title = title;
            Image = image;
            Content = content;
            GenreId = genreId;
        }

        public Article() { }
    }
}
