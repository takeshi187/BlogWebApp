using BlogWebApp.Models;
using BlogWebApp.Services.ArticleServices;
using BlogWebApp.Services.UserServices;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace BlogWebApp.Services.CommentServices
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IArticleService _articleService;
        private readonly IUserService _userService;
        private readonly ILogger _logger;

        public CommentService(ICommentRepository commentRepository, IArticleService articleService, IUserService userService, ILogger logger)
        {
            _commentRepository = commentRepository;
            _articleService = articleService;
            _userService = userService;
            _logger = logger;
        }
        public async Task<Comment> CreateCommentAsync(Comment comment)
        {           
            try
            {
                if (string.IsNullOrWhiteSpace(comment.UserId))
                    throw new ArgumentException("UserId cannot be empty.", nameof(comment.UserId));
                if (comment.ArticleId == Guid.Empty)
                    throw new ArgumentException("ArticleId cannot be empty.", nameof(comment.ArticleId));
                if (string.IsNullOrWhiteSpace(comment.Content))
                    throw new ArgumentException("Content cannot be empty.", nameof(comment.Content));

                var article = await _articleService.GetArticleByIdAsync(comment.ArticleId);
                if (article == null)
                    throw new InvalidOperationException($"Article with id: {comment.ArticleId} not found.");

                var user = await _userService.GetUserByIdAsync(comment.UserId);
                if (user == null)
                    throw new InvalidOperationException($"User with id: {comment.UserId} not found.");

                await _commentRepository.AddAsync(comment);
                return comment;
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid comment data for creation.");
                throw;
            }
            catch(DbUpdateException ex)
            {
                _logger.LogError(ex, $"Database error while adding comment: {comment.CommentId} with content: {comment.Content}");
                throw new InvalidOperationException("Failed to add comment to database.", ex);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error while adding comment: {comment.CommentId} with content: {comment.Content}");
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
            catch(InvalidOperationException ex)
            {
                _logger.LogWarning(ex, $"Comment not found: {commentId}");
                throw;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error while searching comment with id: {commentId}");
                throw;
            }
        }

        public async Task<IEnumerable<Comment?>> GetCommentsByArticleIdAsync(Guid articleId)
        {            
            try
            {
                if (articleId == Guid.Empty)
                    throw new ArgumentException("ArticleId cannot be empty.", nameof(articleId));

                var comments = await _commentRepository.GetByArticleIdAsync(articleId);

                if (comments == null || !comments.Any())
                    throw new InvalidOperationException($"Comments for article with id: {articleId} not found.");

                return comments;
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, $"Comment with article id: {articleId} not found.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error while searching comments with article id: {articleId}");
                throw;
            }
        }

        public async Task<Comment?> UpdateCommentAsync(Comment comment)
        {
            try
            {
                if (comment == null)
                    throw new ArgumentException("Comment cannot be empty.", nameof(comment));
                if (string.IsNullOrWhiteSpace(comment.Content))
                    throw new ArgumentException("Content cannot be empty.", nameof(comment.Content));
                if (string.IsNullOrWhiteSpace(comment.UserId))
                    throw new ArgumentException("UserId cannot be empty.", nameof(comment.UserId));
                if (comment.ArticleId == Guid.Empty)
                    throw new ArgumentException("ArticleId cannot be empty.", nameof(comment.ArticleId));

                var existingComment = await _commentRepository.GetByIdAsync(comment.CommentId);
                if (existingComment == null)
                    throw new InvalidOperationException($"Comment with id: {comment.CommentId} not found.");
                
                existingComment.Content = comment.Content;
                existingComment.UpdatedAt = DateTime.UtcNow;
                await _commentRepository.UpdateAsync(existingComment);
                return existingComment;
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid comment data for updating.");
                throw;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Database error while updating comment: {comment.CommentId}");
                throw new InvalidOperationException("Failed to update comment.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error while updating comment: {comment.CommentId}");
                throw;
            }
        }

        public async Task<bool> DeleteCommentAsync(Guid commentId)
        {            
            try
            {
                if (commentId == Guid.Empty)
                    throw new ArgumentException($"CommentId cannot be empty", nameof(commentId));

                var comment = await _commentRepository.GetByIdAsync(commentId);
                if (comment == null) throw new InvalidOperationException($"Comment with id {commentId} not found.");

                return await _commentRepository.DeleteAsync(comment);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, $"Comment not found: {commentId}");
                throw;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Database error while deleting comment: {commentId}");
                throw new InvalidOperationException("Failed to delete comment.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error while deleting comment: {commentId}");
                throw;
            }
        }
    }
}
