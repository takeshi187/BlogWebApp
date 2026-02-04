using System.ComponentModel.DataAnnotations;

namespace BlogWebApp.Models
{
    public class Genre
    {
        [Key]
        public Guid GenreId { get; private set; }

        [Required]
        [MaxLength(150)]
        public string GenreName { get; private set; } = null!;

        public Genre(string genreName)
        {
            if (string.IsNullOrWhiteSpace(genreName))
                throw new ArgumentException("Genre name cannot be empty.", nameof(genreName));
            GenreId = Guid.NewGuid();
            GenreName = genreName;
        }

        protected Genre() { }
    }
}
