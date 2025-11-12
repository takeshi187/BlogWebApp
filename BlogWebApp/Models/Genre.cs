using System.ComponentModel.DataAnnotations;

namespace BlogWebApp.Models
{
    public class Genre
    {
        [Key]
        public Guid GenreId { get; private set; } = Guid.NewGuid();
        [Required]
        [MaxLength(200)]
        public string GenreName { get; set; } = null!;

        public Genre(string genreName)
        {
            GenreName = genreName;
        }

        // for tests
        public Genre(Guid genreId, string genreName)
        {
            GenreId = genreId;
            GenreName = genreName;
        }

        private Genre() { }
    }
}
