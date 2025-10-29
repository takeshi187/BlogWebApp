using BlogWebApp.Models;
using BlogWebApp.Services.ArticleServices;
using BlogWebApp.Services.UserServices;

namespace BlogWebApp.Services.LikeServices
{
    public class LikeService : ILikeService
    {
        private readonly ILikeRepository _likeRepository;
        private readonly IArticleService _articleService;
        private readonly IUserService _userService;

        public LikeService(ILikeRepository likeRepository, IUserService userService, IArticleService articleService)
        {
            _likeRepository = likeRepository;
            _articleService = articleService;
            _userService = userService;
        }

        public async Task<bool> AddLikeAsync(Guid articleId, string userId)
        {
            if(string.IsNullOrEmpty(userId))
                throw new ArgumentException("UserId cannot be empty.");
            
            if(string.IsNullOrEmpty(articleId.ToString()))
                throw new ArgumentException("ArticleId cannot be empty.");

            var article = await _articleService.GetArticleByIdAsync(articleId);
            if (article == null)
                throw new InvalidOperationException($"Article with id {articleId} not found.");

            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
                throw new InvalidOperationException($"User with id {userId} not found.");

            var alreadyLiked = await _likeRepository.ExistAsync(articleId, userId);
            if(alreadyLiked)
                return false;

            var like = new Like(userId, articleId);
            await _likeRepository.AddAsync(like);
            return true;
        }

        public async Task<bool> DeleteLikeAsync(Guid articleId, string userId)
        {
            var likes = await _likeRepository.GetByArticleIdAsync(articleId);
            if (likes == null)
                return false;

            var like = likes.FirstOrDefault(l => l.UserId == userId);

            if(like == null)
                return false;

            await _likeRepository.DeleteAsync(like);
            return true;
        }

        public async Task<IList<Like>> GetLikesForArticleAsync(Guid articleId)
        {
            return await _likeRepository.GetByArticleIdAsync(articleId);
        }
    }
}
