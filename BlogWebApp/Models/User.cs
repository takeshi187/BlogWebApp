using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogWebApp.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; private set; }

        [Required]
        [MaxLength(200)]
        // uniq
        public string Username { get; set; }

        [Required]
        [MaxLength(200)]
        public string PasswordHash { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(255)]
        // uniq
        public string Email { get; set; }

        [Required]
        public DateOnly CreatedAt { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

        public User(string username, string passwordHash, string email)
        {
            Username = username;
            PasswordHash = passwordHash;
            Email = email;
        }

        // for tests
        public User(int id,  string username, string passwordHash, string email)
        {
            UserId = id;
            Username = username;
            PasswordHash = passwordHash;
            Email = email;
        }

        public User() { }
    }
}
