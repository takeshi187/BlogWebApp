using Microsoft.AspNetCore.Identity;

namespace BlogWebApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    }
}
