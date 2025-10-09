using BlogWebApp.Db;
using BlogWebApp.Models;

namespace BlogWebApp.Services
{
    public class ArticleRepository : IArticleRepository
    {
        private readonly BlogWebAppDbContext _db;

        public ArticleRepository(BlogWebAppDbContext db)
        {
            _db = db;
        }

        public async Task<Article> AddAsync(Article article)
        {
            _db.Articles.Add(article);
            await _db.SaveChangesAsync();
            return article;
        }

        public Task DeleteAsync(int articleId)
        {
            throw new NotImplementedException();
        }

        public Task<IList<Article>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Article> GetByIdAsync(int articleId)
        {
            throw new NotImplementedException();
        }
    }
}
