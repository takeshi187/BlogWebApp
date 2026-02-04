using Microsoft.AspNetCore.Identity;

namespace BlogWebApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        public DateTime CreatedAt { get; private set; }

        public List<Comment> Comments { get; private set; } = new();
        public List<Like> Likes { get; private set; } = new(); 

        public ApplicationUser()
        {
            CreatedAt = DateTime.UtcNow;
        }
    }
}
