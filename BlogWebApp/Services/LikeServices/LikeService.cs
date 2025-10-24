using BlogWebApp.Models;
using BlogWebApp.Services.ArticleServices;

namespace BlogWebApp.Services.LikeServices
{
    public class LikeService : ILikeService
    {
        private readonly ILikeRepository _likeRepository;
        private readonly IArticleRepository _articleRepository;
        //private readonly IUserRepository _userRepository;

        public LikeService(ILikeRepository likeRepository, IArticleRepository articleRepository)
        {
            _likeRepository = likeRepository;
            _articleRepository = articleRepository;
        }

        public async Task<bool> AddLikeAsync(Guid articleId, string userId)
        {
            if(string.IsNullOrEmpty(userId))
                throw new ArgumentException("UserId cannot be empty.");
            
            if(string.IsNullOrEmpty(articleId.ToString()))
                throw new ArgumentException("ArticleId cannot be empty.");

            var article = await _articleRepository.GetByIdAsync(articleId);
            if (article == null)
                throw new InvalidOperationException($"Article with id { articleId} not found.");

            //var user = await _userRepository.GetByIdAsync(userId);
            //if (user == null)
            //    throw new InvalidOperationException($"User with id {articleId} not found.");

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
