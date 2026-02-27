using BlogWebApp.Models;
using Microsoft.AspNetCore.Identity;

namespace BlogWebApp.Services.UserServices
{
    public interface IUserService
    {
        Task<IdentityResult> RegisterAsync(string username, string email, string password);
        Task<bool> LoginAsync(string email, string password, bool rememberMe);
        Task<bool> DeleteUserAsync(string id);
        Task<IReadOnlyList<ApplicationUser>> GetAllUsersAsync();
        Task LogoutAsync();
    }
}
