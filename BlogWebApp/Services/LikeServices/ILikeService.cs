using BlogWebApp.Models;

namespace BlogWebApp.Services.LikeServices
{
    public interface ILikeService
    {
        Task<bool> AddLikeAsync(Guid articleId, string userId);
        Task<bool> DeleteLikeAsync(Guid articleId, string userId);
        Task<IList<Like>>GetLikesForArticleAsync(Guid articleId);
    }
}
