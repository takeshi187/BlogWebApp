using BlogWebApp.Models;
using BlogWebApp.Services.ArticleServices;
using BlogWebApp.Services.UserServices;
using Microsoft.EntityFrameworkCore;

namespace BlogWebApp.Services.LikeServices
{
    public class LikeService : ILikeService
    {
        private readonly ILikeRepository _likeRepository;
        private readonly IArticleService _articleService;
        private readonly IUserService _userService;
        private readonly ILogger<LikeService> _logger;

        public LikeService(ILikeRepository likeRepository, IUserService userService, IArticleService articleService, ILogger<LikeService> logger)
        {
            _likeRepository = likeRepository;
            _articleService = articleService;
            _userService = userService;
            _logger = logger;
        }

        public async Task<bool> AddLikeAsync(Guid articleId, string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                    throw new ArgumentException("UserId cannot be empty.", nameof(userId));

                if (articleId == Guid.Empty)
                    throw new ArgumentException("ArticleId cannot be empty.", nameof(articleId));

                var article = await _articleService.GetArticleByIdAsync(articleId);
                if (article == null)
                    throw new InvalidOperationException($"Article with id {articleId} not found.");

                var user = await _userService.GetUserByIdAsync(userId);
                if (user == null)
                    throw new InvalidOperationException($"User with id {userId} not found.");

                var alreadyLiked = await _likeRepository.ExistAsync(articleId, userId);
                if (alreadyLiked)
                    return false;

                var like = new Like(userId, articleId);
                await _likeRepository.AddAsync(like);
                return true;
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid like data for creation.");
                throw;
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, $"Like with article id: {articleId}, user id: {userId} not found.");
                throw;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Database error while adding like with article id: {articleId}, user id: {userId}");
                throw new InvalidOperationException("Failed to add like to database.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error while adding like on article: {articleId} by user: {userId}");
                throw;
            }
        }

        public async Task<Like?> GetLikeByIdAsync(Guid likeId)
        {
            try
            {
                if (likeId == Guid.Empty)
                    throw new ArgumentException("LikeId cannot be empty.", nameof(likeId));

                var like =  await _likeRepository.GetByIdAsync(likeId);
                if (like == null)
                    throw new InvalidOperationException($"Like with id: {likeId} not found.");

                return like;
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, $"Like not found: {likeId}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error while searching like with id: {likeId}");
                throw;
            }
        }

        public async Task<IEnumerable<Like?>> GetLikesForArticleAsync(Guid articleId)
        {
            try
            {
                if (articleId == Guid.Empty)
                    throw new ArgumentException("ArticleId cannot be empty.", nameof(articleId));

                var likes = await _likeRepository.GetByArticleIdAsync(articleId);

                if (likes == null || !likes.Any())
                    throw new InvalidOperationException($"Likes for article with id: {articleId} not found.");

                return likes;
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, $"Likes with article id: {articleId} not found.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error while searching likes with article id: {articleId}");
                throw;
            }
        }

        public async Task<bool> DeleteLikeAsync(Guid articleId, string userId)
        {
            try
            {
                if (articleId == Guid.Empty)
                    throw new ArgumentException("articleId cannot be empty.", nameof(articleId));
                if (userId == null)
                    throw new ArgumentException("userId cannot be empty.", nameof(userId));
                
                var likes = await _likeRepository.GetByArticleIdAsync(articleId);
                if (likes == null)
                    return false;

                var like = likes.FirstOrDefault(l => l.UserId == userId);

                if (like == null)
                    return false;

                await _likeRepository.DeleteAsync(like);
                return true;
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, $"Like with article id: {articleId}, user id: {userId} not found.");
                throw;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Database error while deleting like with article id: {articleId}, user id: {userId}");
                throw new InvalidOperationException("Failed to delete like.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error while deleting like: with article id: {articleId}, user id: {userId}");
                throw;
            }
        }
    }
}
