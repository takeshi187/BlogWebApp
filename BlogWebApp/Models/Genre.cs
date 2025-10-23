using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogWebApp.Models
{
    public class Genre
    {
        [Key]
        public Guid GenreId { get; private set; } = Guid.NewGuid();
        [Required]
        [MaxLength(200)]
        public string GenreName { get; set; }
    }
}
