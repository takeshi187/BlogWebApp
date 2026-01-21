using BlogWebApp.Models;

namespace BlogWebApp.Services.LikeServices
{
    public interface ILikeService
    {
        Task ToggleLikeAsync(Guid articleId, string userId);
        Task<Like?> GetLikeByIdAsync(Guid likeId);
        Task DeleteLikesByArticleIdAsync(Guid articleId);
        Task DeleteLikesByUserIdAsync(string userId);
    }
}
