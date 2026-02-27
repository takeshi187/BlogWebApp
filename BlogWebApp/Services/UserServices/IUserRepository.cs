using BlogWebApp.Models;
using Microsoft.AspNetCore.Identity;

namespace BlogWebApp.Services.UserServices
{
    public interface IUserRepository
    {
        Task<IdentityResult> AddAsync(ApplicationUser user, string passwordHash);
        Task<ApplicationUser?> GetByIdAsync(string userId);
        Task<ApplicationUser?> GetByEmailAsync(string email);
        Task<IdentityResult> DeleteAsync(ApplicationUser user);
        Task<IReadOnlyList<ApplicationUser>> GetAllAsync();
    }
}
