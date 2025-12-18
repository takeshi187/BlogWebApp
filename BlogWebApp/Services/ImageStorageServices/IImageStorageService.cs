namespace BlogWebApp.Services.FileStorageServices
{
    public interface IImageStorageService
    {
        Task<string?> SaveArticleImageAsync(IFormFile? file);
    }
}
