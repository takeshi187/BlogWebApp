using System.ComponentModel.DataAnnotations;

namespace BlogWebApp.Models
{
    public class Article
    {
        [Key]
        public int ArticleId { get; set; }
        [Required]
        public string Title { get; set; }
        public string? Image { get; set; }
        [Required]
        public string Content { get; set; }
        [Required]
        public int Likes { get; set; }
        [Required]
        public int GenreId { get; set; }
        [Required]
        public DateOnly CreatedAt { get; set; }
        public DateOnly? UpdatedAt { get; set; }


        public Article(string title, string image, string content, int likes, int genreId, DateOnly createdAt, DateOnly? updatedAt)
        {
            Title = title;
            Image = image;
            Content = content;
            Likes = likes;
            GenreId = genreId;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
        }
    }
}
