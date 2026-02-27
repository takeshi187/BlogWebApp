using BlogWebApp.Db;
using BlogWebApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BlogWebApp.Services.UserServices
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly BlogWebAppDbContext _db;

        public UserRepository(UserManager<ApplicationUser> userManager, BlogWebAppDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        public async Task<IdentityResult> AddAsync(ApplicationUser user, string password)
        {
            return await _userManager.CreateAsync(user, password);
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

        public async Task<IReadOnlyList<ApplicationUser>> GetAllAsync()
        {
            return await _db.Users.ToListAsync();
        }
    }
}
