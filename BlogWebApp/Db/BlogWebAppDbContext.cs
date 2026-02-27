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
        public DbSet<Like> Likes { get; set; }
        public DbSet<Genre> Genres { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Article>()
                .HasIndex(a => a.GenreId);

            builder.Entity<Article>()
                .HasIndex(a => a.CreatedAt);

            builder.Entity<Comment>()
                .HasIndex(c => c.ArticleId);

            builder.Entity<Comment>()
                .HasIndex(c => c.UserId);

            builder.Entity<Like>()
                .HasIndex(l => new { l.UserId, l.ArticleId })
                .IsUnique();

            builder.Entity<Like>()
                .HasIndex(l => l.ArticleId);

            builder.Entity<Like>()
                .HasIndex(l => l.UserId);

            builder.Entity<Genre>()
                .HasIndex(g => g.GenreName)
                .IsUnique();

            builder.Entity<ApplicationUser>()
                .HasIndex(u => u.NormalizedEmail)
                .IsUnique();
        }
    }
}
