using BlogWebApp.Models;
using Microsoft.AspNetCore.Identity;

namespace BlogWebApp.Services.UserServices
{
    public interface IUserRepository
    {
        Task<ApplicationUser> GetByIdAsync(string userId);
        Task<ApplicationUser> GetByEmailAsync(string email);
        Task<IdentityResult> CreateAsync(ApplicationUser user, string passwordHash);
        Task<IdentityResult> DeleteAsync(ApplicationUser user);
        Task<bool> CheckPasswordAsync(ApplicationUser user, string passwordHash);
    }
}
