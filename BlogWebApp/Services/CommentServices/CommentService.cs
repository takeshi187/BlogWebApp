using BlogWebApp.Models;
using BlogWebApp.Services.ArticleServices;
using BlogWebApp.Services.UserServices;

namespace BlogWebApp.Services.CommentServices
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IArticleService _articleService;
        private readonly IUserService _userService;

        public CommentService(ICommentRepository commentRepository, IArticleService articleService, IUserService userService)
        {
            _commentRepository = commentRepository;
            _articleService = articleService;
            _userService = userService;
        }
        public async Task<Comment> AddCommentAsync(Comment comment)
        {
            if (string.IsNullOrEmpty(comment.UserId))
                throw new ArgumentException("UserId cannot be empty.");

            if (string.IsNullOrEmpty(comment.ArticleId.ToString()))
                throw new ArgumentException("ArticleId cannot be empty.");

            if (string.IsNullOrEmpty(comment.Content))
                throw new ArgumentException("Content cannot be empty.");

            var article = await _articleService.GetArticleByIdAsync(comment.ArticleId);
            if (article == null)
                throw new InvalidOperationException($"Article with id {comment.ArticleId} not found.");

            var user = await _userService.GetUserByIdAsync(comment.UserId);
            if (user == null)
                throw new InvalidOperationException($"User with id {comment.UserId} not found.");

            await _commentRepository.AddAsync(comment);
            return comment;
        }

        public async Task<bool> DeleteCommentAsync(Guid commentId)
        {
            var comment = await _commentRepository.GetByIdAsync(commentId);
            if (comment == null)
                throw new InvalidOperationException($"Comment with id {commentId} not found.");

            await _commentRepository.DeleteAsync(comment);
            return true;
        }

        public async Task<Comment> GetCommentByIdAsync(Guid commentId)
        {
            var result = await _commentRepository.GetByIdAsync(commentId);
            if (result == null)
                throw new InvalidOperationException($"Comment with id {commentId} not found.");

            return result;
        }

        public async Task<IList<Comment>> GetCommentsByArticleAsync(Guid articleId)
        {
            return await _commentRepository.GetByArticleIdAsync(articleId);
        }

        public async Task<bool> UpdateCommentAsync(Guid commentId, string newContent)
        {
            var comment = await _commentRepository.GetByIdAsync(commentId);
            if(comment == null)
                return false;

            comment.Content = newContent;
            await _commentRepository.UpdateAsync(comment);
            return true;
        }
    }
}
