using BlogWebApp.Models;
using Microsoft.AspNetCore.Identity;

namespace BlogWebApp.Services.UserServices
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserRepository(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IdentityResult> AddAsync(ApplicationUser user, string passwordHash)
        {
            return await _userManager.CreateAsync(user, passwordHash);
        }

        public async Task<ApplicationUser?> GetByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<ApplicationUser?> GetByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<IdentityResult> DeleteAsync(ApplicationUser user)
        {
            return await _userManager.DeleteAsync(user);
        }

        public async Task<bool> CheckPasswordAsync(ApplicationUser user, string passwordHash)
        {
            return await _userManager.CheckPasswordAsync(user, passwordHash);
        }
    }
}
