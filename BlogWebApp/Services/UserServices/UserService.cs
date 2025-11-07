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
            try
            {
                if(string.IsNullOrWhiteSpace(username))
                    throw new ArgumentException("Username cannot be empty.", nameof(username));
                if (string.IsNullOrWhiteSpace(email))
                    throw new ArgumentException("Email cannot be empty.", nameof(email));
                if (string.IsNullOrWhiteSpace(password))
                    throw new ArgumentException("Password cannot be empty.", nameof(password));

                var existingUser = await _userRepository.GetByEmailAsync(email);
                if (existingUser != null)
                    return IdentityResult.Failed(new IdentityError { Description = "User already exist" });

                var user = new ApplicationUser { UserName = username, Email = email };
                var result = await _userRepository.CreateAsync(user, password);

                if (!result.Succeeded)
                    _logger.LogWarning($"Failed to register user: {email}. Errors{string.Join(", ", result.Errors.Select(e => e.Description))}");

                return result;
            }
            catch(ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid registration data.");
                throw;
            }
            catch(InvalidOperationException ex)
            {
                _logger.LogWarning(ex, $"Failed to register user with email: {email}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error while registration for: {email}");
                throw;
            }            
        }

        public async Task<bool> LoginAsync(string email, string password)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                    throw new ArgumentException("Email cannot be empty", nameof(email));
                if (string.IsNullOrWhiteSpace(password))
                    throw new ArgumentException("Password cannot be empty", nameof(password));

                var user = await _userRepository.GetByEmailAsync(email);
                if (user == null)
                    throw new InvalidOperationException($"User with email: {email} not found.");

                var result = await _signInManager.PasswordSignInAsync(user, password, isPersistent: false, lockoutOnFailure: false);

                return result.Succeeded;
            }
            catch(ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid login data.");
                throw;
            }
            catch(InvalidOperationException ex)
            {
                _logger.LogWarning(ex, $"Login failed for: {email}");
                throw;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error while login for: {email}");
                throw;
            }
        }

        public async Task<ApplicationUser?> GetUserByEmailAsync(string email)
        {
            try
            {
                if(string.IsNullOrWhiteSpace(email))
                    throw new ArgumentException("Email cannot be empty.", nameof(email));

                var result = await _userRepository.GetByEmailAsync(email);
                if (result == null) throw new InvalidOperationException($"User with email: {email} not found.");

                return result;               
            }
            catch(ArgumentException ex)
            {
                _logger.LogWarning(ex, $"User with email: {email} not found.");
                throw;
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, $"User with email: {email} not found.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error while searching user with email: {email}");
                throw;
            }
        }

        public async Task<ApplicationUser?> GetUserByIdAsync(string userId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                    throw new ArgumentException("UserId cannot be empty.", nameof(userId));

                var result = await _userRepository.GetByIdAsync(userId);
                if (result == null) throw new InvalidOperationException($"User with userId: {userId} not found.");

                return result;
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, $"User with userId: {userId} not found.");
                throw;
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, $"Failed to get user with userId: {userId}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error while searching user with userId: {userId}");
                throw;
            }
        }

        public async Task<IdentityResult> DeleteUserAsync(string userId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                    throw new ArgumentException("UserId cannot be empty.", nameof(userId));

                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                    return IdentityResult.Failed(new IdentityError { Description = "User not found" });

                return await _userRepository.DeleteAsync(user);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, $"User with userId: {userId} not found.");
                throw;
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, $"Failed to delete user with userId: {userId}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error while deleting user with userId: {userId}");
                throw;
            }
        }

        public async Task LogoutAsync()
        {
            try
            {
                await _signInManager.SignOutAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while logout.");
                throw new InvalidOperationException("Failed to logout.", ex);
            }
        }
    }
}
