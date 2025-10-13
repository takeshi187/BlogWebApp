using BlogWebApp.Models;

namespace BlogWebApp.Services
{
    public interface IArticleRepository
    {
        Task<Article> AddAsync(Article article);
        Task<Article> GetByIdAsync(int articleId);
        Task<IList<Article>> GetAllAsync();
        Task<Article> UpdateAsync(Article article);
        Task DeleteAsync(int articleId);
    }
}
