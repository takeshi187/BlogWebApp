using BlogWebApp.Models;
using BlogWebApp.Services.ArticleServices;

namespace BlogWebApp.Services.FileStorageServices
{
    public class ImageStorageService : IImageStorageService
    {
        private static readonly long _maxFileSize = 5 * 1024 * 600; // 5 mb
        private static readonly string[] _allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<ImageStorageService> _logger;

        public ImageStorageService(IWebHostEnvironment env, ILogger<ImageStorageService> logger)
        {
            _env = env;
            _logger = logger;
        }

        public async Task<string?> SaveArticleImageAsync(IFormFile? file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return null;

                Validate(file);

                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                var fileName = GenerateFileName(extension);

                var directory = Path.Combine(_env.WebRootPath, "img", "ArticleImages");
                Directory.CreateDirectory(directory);

                var fullPath = Path.Combine(directory, fileName);

                await using var sream = new FileStream(fullPath, FileMode.Create);
                await file.CopyToAsync(sream);

                return $"/img/ArticleImages/{fileName}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error while adding image");
                throw;
            }           
        }

        private static void Validate(IFormFile file)
        {           
            if (file.Length > _maxFileSize)
                throw new InvalidOperationException("The image size must not exceed 5 MB.");

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_allowedExtensions.Contains(extension))
                throw new ArgumentException("Invalid image format.");
        }

        private static string GenerateFileName(string extension)
        {
            return $"{Guid.NewGuid():N}{extension}";
        }
    }
}
