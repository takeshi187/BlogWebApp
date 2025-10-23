using BlogWebApp.Models;

namespace BlogWebApp.Services
{
    public interface IArticleService
    {
        Task<Article> CreateArticleAsync(Article article);
        Task<Article> GetArticleByIdAsync(Guid ArticleId);
        Task<IList<Article>> GetAllArticlesAsync();
        Task<Article> UpdateArticleAsync(Article article);
        Task DeleteArticleAsync(Guid articleId);
    }
}
