using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogWebApp.Models
{
    public class Article
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ArticleId { get; set; }
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }
        public string? Image { get; set; }
        [Required]
        public string Content { get; set; }
        public int Likes { get; set; } = 0;
        [ForeignKey(nameof(Genre.GenreId))]
        [Required]
        public int GenreId { get; set; }
        [Required]
        public DateOnly CreatedAt { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
        public DateOnly? UpdatedAt { get; set; }

        public List<Comment> Comments { get; set; } = new();
        public List<Like> LikeList { get; set; } = new();

        public Article() { }

        public Article(string title, string image, string content, int genreId)
        {
            Title = title;
            Image = image;
            Content = content;
            GenreId = genreId;
        }
    }
}
