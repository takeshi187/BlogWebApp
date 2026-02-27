using BlogWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogWebApp.Services.CommentServices
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly ILogger<CommentService> _logger;

        public CommentService(ICommentRepository commentRepository, ILogger<CommentService> logger)
        {
            _commentRepository = commentRepository;
            _logger = logger;
        }

        public async Task CreateCommentAsync(Guid articleId, string userId, string content)
        {
            try
            {
                var comment = new Comment(content, userId, articleId);
                await _commentRepository.AddAsync(comment);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Database error while adding comment. ArticleId={articleId}, UserId={userId}");
                throw;
            }
        }

        public async Task<Comment?> GetCommentByIdAsync(Guid commentId)
        {
            if (commentId == Guid.Empty)
                throw new ArgumentException("CommentId cannot be empty.", nameof(commentId));

            return await _commentRepository.GetByIdAsync(commentId);
        }

        public async Task<bool> DeleteCommentsByArticleIdAsync(Guid articleId)
        {
            try
            {
                if (articleId == Guid.Empty)
                    throw new ArgumentException($"ArticleId cannot be empty", nameof(articleId));

                var comments = await _commentRepository.GetByArticleIdAsync(articleId);

                if (comments == null || !comments.Any())
                    return false;

                await _commentRepository.DeleteRangeAsync(comments);
                return true;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Database error while deleting comments for article: {articleId}");
                throw;
            }
        }

        public async Task<bool> DeleteCommentsByUserIdAsync(string userId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                    throw new ArgumentException($"UserId cannot be empty", nameof(userId));

                var comments = await _commentRepository.GetByUserIdAsync(userId);

                if (comments == null || !comments.Any())
                    return false;

                await _commentRepository.DeleteRangeAsync(comments);
                return true;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Database error while deleting comments for user: {userId}");
                throw;
            }
        }

        public async Task<bool> DeleteCommentByIdAsync(Guid commentId)
        {
            try
            {
                if (commentId == Guid.Empty)
                    throw new ArgumentException($"CommentId cannot be empty", nameof(commentId));

                var comment = await _commentRepository.GetByIdAsync(commentId);

                if (comment == null)
                    return false;

                await _commentRepository.DeleteAsync(comment);
                return true;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Database error while deleting comment: {commentId}");
                throw;
            }
        }
    }
}
