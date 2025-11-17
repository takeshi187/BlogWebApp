using BlogWebApp.Models;

namespace BlogWebApp.Services.ArticleServices
{
    public interface IArticleService
    {
        Task<Article> CreateArticleAsync(Article article);
        Task<Article?> GetArticleByIdAsync(Guid ArticleId);
        Task<IEnumerable<Article?>> GetAllArticlesAsync();
        Task<Article?> UpdateArticleAsync(Article article);
        Task<bool> DeleteArticleAsync(Guid articleId);
    }
}
