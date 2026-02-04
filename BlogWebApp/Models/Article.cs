using System.ComponentModel.DataAnnotations;

namespace BlogWebApp.Models
{
    public class Article
    {
        [Key]
        public Guid ArticleId { get; private set; }

        [Required]
        [MaxLength(300)]
        public string Title { get; private set; } = null!;

        public string? Image { get; private set; }

        [Required]
        public string Content { get; private set; } = null!;

        [Required]
        public Guid GenreId { get; private set; }
        public Genre Genre { get; private set; } = null!;

        [Required]
        public DateTime CreatedAt { get; private set; }

        public DateTime? UpdatedAt { get; private set; }

        public List<Comment> Comments { get; private set; } = new();
        public List<Like> Likes { get; private set; } = new();

        public Article(string title, string? image, string content, Guid genreId)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty.", nameof(title));
            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("Content cannot be empty.", nameof(content));
            if (genreId == Guid.Empty)
                throw new ArgumentException("GenreId cannot be empty.", nameof(genreId));

            ArticleId = Guid.NewGuid();
            Title = title;
            Image = image;
            Content = content;
            GenreId = genreId;
            CreatedAt = DateTime.UtcNow;
        }

        public Article(string title, string? image, string content, Genre genre)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty.", nameof(title));
            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("Content cannot be empty.", nameof(content));
            if (genre == null)
                throw new ArgumentException("Genre cannot be empty.", nameof(genre));

            ArticleId = Guid.NewGuid();
            Title = title;
            Image = image;
            Content = content;
            Genre = genre;
            GenreId = genre.GenreId;
            CreatedAt = DateTime.UtcNow;
        }

        protected Article() { }

        public void Update(string title, string? image, string content, Guid genreId)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty.", nameof(title));

            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("Content cannot be empty.", nameof(content));

            if (genreId == Guid.Empty)
                throw new ArgumentException("GenreId cannot be empty.", nameof(genreId));

            Title = title;
            Image = image;
            Content = content;
            GenreId = genreId;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
