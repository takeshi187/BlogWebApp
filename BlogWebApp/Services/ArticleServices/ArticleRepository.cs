using BlogWebApp.Db;
using BlogWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogWebApp.Services.ArticleServices
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
            await _db.Articles.AddAsync(article);
            await _db.SaveChangesAsync();
            return article;
        }

        public async Task<Article?> GetByIdAsync(Guid articleId)
        {
            return await _db.Articles.FindAsync(articleId);
        }

        public async Task<bool> UpdateAsync(Article article)
        { 
            _db.Articles.Update(article);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(Article article)
        {
            _db.Articles.Remove(article);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
