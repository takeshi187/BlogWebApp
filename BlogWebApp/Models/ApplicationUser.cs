using Microsoft.AspNetCore.Identity;

namespace BlogWebApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        public DateOnly CreatedAt { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    }
}
