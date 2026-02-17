using BlogWebApp.Db;
using BlogWebApp.Models;
using BlogWebApp.Services.ArticleServices;
using BlogWebApp.Services.CommentServices;
using BlogWebApp.Services.FileStorageServices;
using BlogWebApp.Services.GenreServices;
using BlogWebApp.Services.LikeServices;
using BlogWebApp.Services.UserServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<BlogWebAppDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IArticleService, ArticleService>();
builder.Services.AddScoped<IArticleRepository, ArticleRepository>();
builder.Services.AddScoped<ILikeService, LikeService>();
builder.Services.AddScoped<ILikeRepository, LikeRepository>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<IGenreService, GenreService>();
builder.Services.AddScoped<IGenreRepository, GenreRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IImageStorageService, ImageStorageService>();
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true;
})
    .AddEntityFrameworkStores<BlogWebAppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.SlidingExpiration = true; // Продление Cookie
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // https only
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

await DatabaseInitializer.InitializeAsync(app.Services);
app.Run();
