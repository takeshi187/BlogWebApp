using BlogWebApp.Models;
using BlogWebApp.Validators;
using Microsoft.EntityFrameworkCore;

namespace BlogWebApp.Services.LikeServices
{
    public class LikeService : ILikeService
    {
        private readonly ILikeRepository _likeRepository;
        private readonly ILogger<LikeService> _logger;

        public LikeService(ILikeRepository likeRepository, ILogger<LikeService> logger)
        {
            _likeRepository = likeRepository;
            _logger = logger;
        }

        public async Task ToggleLikeAsync(Guid articleId, string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                    throw new ArgumentException("UserId cannot be empty.", nameof(userId));

                if (articleId == Guid.Empty)
                    throw new ArgumentException("ArticleId cannot be empty.", nameof(articleId));

                var existingLike = await _likeRepository.GetByArticleIdAndUserIdAsync(articleId, userId);

                if (existingLike != null)
                {
                    await _likeRepository.DeleteAsync(existingLike);
                    return;
                }

                var like = new Like(userId, articleId);
                await _likeRepository.AddAsync(like);
            }
            catch (DbUpdateException ex) when (ex.IsUniqueViolation())
            {
                _logger.LogWarning(ex, $"Race condition while adding like. ArticleId={articleId}, UserId={userId}");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Unexpected database error while toggle like. ArticleId={articleId}, UserId={userId}");
                throw;
            }
        }

        public async Task<Like?> GetLikeByIdAsync(Guid likeId)
        {
            if (likeId == Guid.Empty)
                throw new ArgumentException("LikeId cannot be empty.", nameof(likeId));

            return await _likeRepository.GetByIdAsync(likeId);
        }

        public async Task<bool> DeleteLikesByArticleIdAsync(Guid articleId)
        {
            try
            {
                if (articleId == Guid.Empty)
                    throw new ArgumentException($"ArticleId cannot be empty", nameof(articleId));

                var likes = await _likeRepository.GetByArticleIdAsync(articleId);
                if (likes == null || !likes.Any())
                    return false;

                await _likeRepository.DeleteRangeAsync(likes);
                return true;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Database error while deleting likes for article: {articleId}");
                throw;
            }
        }

        public async Task<bool> DeleteLikesByUserIdAsync(string userId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                    throw new ArgumentException($"UserId cannot be empty", nameof(userId));

                var likes = await _likeRepository.GetByUserIdAsync(userId);
                if (likes == null || !likes.Any())
                    return false;

                await _likeRepository.DeleteRangeAsync(likes);
                return true;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Database error while deleting likes for user: {userId}");
                throw;
            }
        }
    }
}
