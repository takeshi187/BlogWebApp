using System.ComponentModel.DataAnnotations;

namespace BlogWebApp.Models
{
    public class Article
    {
        [Key]
        public Guid ArticleId { get; private set; } = Guid.NewGuid();

        [Required]
        [MaxLength(300)]
        public string Title { get; set; } = null!;

        public string? Image { get; set; }

        [Required]
        public string Content { get; set; } = null!;

        [Required]
        public Guid GenreId { get; set; }
        public Genre Genre { get; set; } = null!;

        [Required]
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public List<Comment> Comments { get; set; } = new();
        public List<Like> Likes { get; set; } = new();

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

        private Article() { }
    }
}
