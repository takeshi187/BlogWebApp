using BlogWebApp.Db;
using BlogWebApp.Models;
using Microsoft.EntityFrameworkCore;

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

        public async Task DeleteAsync(int articleId)
        {
            var article = await GetByIdAsync(articleId);
            if(article != null)
            {
                _db.Articles.Remove(article);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<IList<Article>> GetAllAsync()
        {
            return await _db.Articles.ToListAsync();
        }

        public async Task<Article> GetByIdAsync(int articleId)
        {
            return await _db.Articles.FirstOrDefaultAsync(a => a.ArticleId == articleId);
        }

        public async Task<Article> UpdateAsync(Article article)
        {
            _db.Articles.Update(article);
            await _db.SaveChangesAsync();
            return article;
        }
    }
}
