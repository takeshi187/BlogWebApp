using BlogWebApp.Models;
using BlogWebApp.Services.ArticleServices;
using BlogWebApp.Services.UserServices;
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
                throw new InvalidOperationException("Failed to add comment to database.", ex);
            }
        }

        public async Task<Comment?> GetCommentByIdAsync(Guid commentId)
        {
            if (commentId == Guid.Empty)
                throw new ArgumentException("CommentId cannot be empty.", nameof(commentId));

            var comment = await _commentRepository.GetByIdAsync(commentId);
            if (comment == null)
                throw new InvalidOperationException($"Comment with id: {commentId} not found.");

            return comment;
        }

        public async Task DeleteCommentsByArticleIdAsync(Guid articleId)
        {
            try
            {
                if (articleId == Guid.Empty)
                    throw new ArgumentException($"ArticleId cannot be empty", nameof(articleId));

                var comments = await _commentRepository.GetByArticleIdAsync(articleId);

                if (comments == null || !comments.Any())
                    return;

                await _commentRepository.DeleteRangeAsync(comments);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Database error while deleting comments for article: {articleId}");
                throw new InvalidOperationException("Failed to delete comments.", ex);
            }
        }

        public async Task DeleteCommentsByUserIdAsync(string userId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                    throw new ArgumentException($"UserId cannot be empty", nameof(userId));

                var comments = await _commentRepository.GetByUserIdAsync(userId);

                if (comments == null || !comments.Any())
                    return;

                await _commentRepository.DeleteRangeAsync(comments);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Database error while deleting comments for user: {userId}");
                throw new InvalidOperationException("Failed to delete comments.", ex);
            }
        }
    }
}
