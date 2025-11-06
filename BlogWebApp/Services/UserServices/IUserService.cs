using BlogWebApp.Models;
using Microsoft.AspNetCore.Identity;

namespace BlogWebApp.Services.UserServices
{
    public interface IUserService
    {
        Task<IdentityResult> RegisterAsync(string username, string email, string password);
        Task<bool> LoginAsync(string email, string password);
        Task<ApplicationUser?> GetUserByEmailAsync(string email);
        Task<ApplicationUser?> GetUserByIdAsync(string userId);
        Task<IdentityResult> DeleteUserAsync(string id);
        Task LogoutAsync();        
    }
}
