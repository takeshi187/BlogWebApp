using Microsoft.AspNetCore.Identity;

namespace BlogWebApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        public DateOnly CreatedAt { get; private set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    }
}
