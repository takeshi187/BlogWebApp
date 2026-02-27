using BlogWebApp.Models;
using Microsoft.AspNetCore.Identity;

namespace BlogWebApp.Services.UserServices
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, SignInManager<ApplicationUser> signInManager, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _signInManager = signInManager;
            _logger = logger;
        }

        public async Task<IdentityResult> RegisterAsync(string username, string email, string password)
        {

            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username cannot be empty.", nameof(username));
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty.", nameof(email));
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be empty.", nameof(password));

            var user = new ApplicationUser { UserName = username, Email = email };
            var result = await _userRepository.AddAsync(user, password);

            if (!result.Succeeded)
                _logger.LogWarning($"Failed to register user: {email}. Errors{string.Join(", ", result.Errors.Select(e => e.Description))}");

            return result;
        }

        public async Task<bool> LoginAsync(string email, string password, bool rememberMe)
        {

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty", nameof(email));
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be empty", nameof(password));

            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
                return false;

            var result = await _signInManager.PasswordSignInAsync(user, password, rememberMe, lockoutOnFailure: false);
            return result.Succeeded;
        }

        public async Task<bool> DeleteUserAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("UserId cannot be empty.", nameof(userId));

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return false;

            var result = await _userRepository.DeleteAsync(user);
            if (!result.Succeeded)
                _logger.LogWarning($"Failed to delete user: {userId}. Errors{string.Join(", ", result.Errors.Select(e => e.Description))}");

            return true;
        }

        public async Task<IReadOnlyList<ApplicationUser>> GetAllUsersAsync()
        {
            try
            {
                return await _userRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while searching users.");
                throw;
            }
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }
    }
}
