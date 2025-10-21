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
        public int UserRoleId { get; set; }
        [ForeignKey(nameof(UserRoleId))]
        public UserRole UserRole { get; set; }       

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

        public User(int userRoleId, string username, string passwordHash, string email)
        {
            UserRoleId = userRoleId;
            Username = username;
            PasswordHash = passwordHash;
            Email = email;
        }

        // for tests
        public User(int id, int userRoleId,  string username, string passwordHash, string email)
        {
            UserId = id;
            UserRoleId = userRoleId;
            Username = username;
            PasswordHash = passwordHash;
            Email = email;
        }

        public User() { }
    }
}
