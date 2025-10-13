using BlogWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogWebApp.Db
{
    public class BlogWebAppDbContext : DbContext
    {
        public BlogWebAppDbContext(DbContextOptions<BlogWebAppDbContext> options) : base(options) { }
        
        public DbSet<Article> Articles { get; set; }
    }
}
