using BlogWebApp.Models;

namespace BlogWebApp.Services.ArticleServices
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
