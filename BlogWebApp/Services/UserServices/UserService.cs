using BlogWebApp.Models;
using Microsoft.AspNetCore.Identity;

namespace BlogWebApp.Services.UserServices
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public UserService(IUserRepository userRepository, SignInManager<ApplicationUser> signInManager)
        {
            _userRepository = userRepository;
            _signInManager = signInManager;
        }

        public async Task<IdentityResult> DeleteUserAsync(string id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return IdentityResult.Failed(new IdentityError { Description = "User not found" });

            return await _userRepository.DeleteAsync(user);
        }

        public async Task<ApplicationUser> GetUserByEmailAsync(string email)
        {
            if (email == null) throw new InvalidOperationException("Invalid email");
            return await _userRepository.GetByEmailAsync(email);
        }

        public async Task<ApplicationUser> GetUserByIdAsync(string userId)
        {
            if (userId == null) throw new InvalidOperationException("Invalid email");
            return await _userRepository.GetByIdAsync(userId);
        }

        public async Task<bool> LoginAsync(string email, string password)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
                return false;

            var result = await _signInManager.PasswordSignInAsync(user, password, isPersistent: false, lockoutOnFailure:false);
            return result.Succeeded;
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<IdentityResult> RegisterAsync(string username, string email, string password)
        {
            var existingUser = await _userRepository.GetByEmailAsync(email);
            if (existingUser != null)
                return IdentityResult.Failed(new IdentityError { Description = "User already exist"});

            var user = new ApplicationUser { UserName = username, Email = email };
            return await _userRepository.CreateAsync(user, password);
        }
    }
}
