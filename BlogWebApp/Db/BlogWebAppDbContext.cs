using BlogWebApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BlogWebApp.Db
{
    public class BlogWebAppDbContext : IdentityDbContext<ApplicationUser>
    {
        public BlogWebAppDbContext(DbContextOptions<BlogWebAppDbContext> options) : base(options) { }
        
        public DbSet<Article> Articles { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Like> Like { get; set; }
        public DbSet<Genre> Genres { get; set; }
    }
}
