using BlogWebApp.Models;

namespace BlogWebApp.Services.ArticleServices
{
    public interface IArticleService
    {
        Task CreateArticleAsync(string title, string? image, string content, Guid genreId);
        Task<Article?> GetArticleByIdAsync(Guid ArticleId);
        Task<IReadOnlyList<Article>> GetAllArticlesAsync();
        Task<bool> UpdateArticleAsync(Guid articleId, string title, string? image, string content, Guid genreId);
        Task<bool> DeleteArticleAsync(Guid articleId);
    }
}
