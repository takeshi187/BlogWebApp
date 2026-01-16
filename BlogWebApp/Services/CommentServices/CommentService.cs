using BlogWebApp.Models;
using BlogWebApp.Services.ArticleServices;
using BlogWebApp.Services.UserServices;
using Microsoft.EntityFrameworkCore;

namespace BlogWebApp.Services.CommentServices
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IArticleService _articleService;
        private readonly IUserService _userService;
        private readonly ILogger<CommentService> _logger;

        public CommentService(ICommentRepository commentRepository, IArticleService articleService, IUserService userService, ILogger<CommentService> logger)
        {
            _commentRepository = commentRepository;
            _articleService = articleService;
            _userService = userService;
            _logger = logger;
        }

        public async Task<Comment> CreateCommentAsync(Guid articleId, string userId, string content)
        {
            try
            {
                if (articleId == Guid.Empty)
                    throw new ArgumentException("ArticleId cannot be empty.", nameof(articleId));
                if (string.IsNullOrWhiteSpace(userId))
                    throw new ArgumentException("UserId cannot be empty.", nameof(userId));
                if (string.IsNullOrWhiteSpace(content))
                    throw new ArgumentException("Content cannot be empty.", nameof(content));

                var article = await _articleService.GetArticleByIdAsync(articleId);
                if (article == null)
                    throw new InvalidOperationException($"Article with id: {articleId} not found.");

                var user = await _userService.GetUserByIdAsync(userId);
                if (user == null)
                    throw new InvalidOperationException($"User with id: {userId} not found.");

                var comment = new Comment(content, userId, articleId);
                await _commentRepository.AddAsync(comment);
                return comment;
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid comment data for creation.");
                throw;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Database error while adding comment with content: {content}");
                throw new InvalidOperationException("Failed to add comment to database.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error while adding comment with content: {content}");
                throw;
            }
        }

        public async Task<Comment?> GetCommentByIdAsync(Guid commentId)
        {
            try
            {
                if (commentId == Guid.Empty)
                    throw new ArgumentException("CommentId cannot be empty.", nameof(commentId));

                var result = await _commentRepository.GetByIdAsync(commentId);
                if (result == null)
                    throw new InvalidOperationException($"Comment with id: {commentId} not found.");

                return result;
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, $"Comment not found: {commentId}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error while searching comment with id: {commentId}");
                throw;
            }
        }            

        public async Task<bool> DeleteCommentsByArticleIdAsync(Guid articleId)
        {
            try
            {
                if (articleId == Guid.Empty)
                    throw new ArgumentException($"ArticleId cannot be empty", nameof(articleId));

                var comments = await _commentRepository.GetByArticleIdAsync(articleId);

                if (comments == null || !comments.Any())
                {
                    _logger.LogInformation($"No one comments not found for article: {articleId}. Skipping delete.");
                    return false;
                }

                return await _commentRepository.DeleteRangeAsync(comments);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Database error while deleting comments for article: {articleId}");
                throw new InvalidOperationException("Failed to delete comments.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error while deleting comments for article: {articleId}");
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
                {
                    _logger.LogInformation($"No one comments not found for user: {userId}. Skipping delete.");
                    return false;
                }

                return await _commentRepository.DeleteRangeAsync(comments);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Database error while deleting comments for user: {userId}");
                throw new InvalidOperationException("Failed to delete comments.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error while deleting comments for user: {userId}");
                throw;
            }
        }
    }
}
