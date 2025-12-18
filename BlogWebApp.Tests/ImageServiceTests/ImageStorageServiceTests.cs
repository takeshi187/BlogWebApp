using BlogWebApp.Services.FileStorageServices;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogWebApp.Tests.ImageServiceTests
{
    [TestFixture]
    public class ImageStorageServiceTests
    {
        private Mock<IWebHostEnvironment> _envMock;
        private Mock<ILogger<ImageStorageService>> _loggerMock;
        private IImageStorageService _imageStorageService;

        [SetUp]
        public void SetUp()
        {
            _envMock = new Mock<IWebHostEnvironment>();
            _loggerMock = new Mock<ILogger<ImageStorageService>>();
            _envMock.Setup(e => e.WebRootPath).Returns("wwwroot");

            _imageStorageService = new ImageStorageService(_envMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task SaveImageAsync_ShouldSaveImage_WhenValidFile()
        {
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.Length).Returns(1024);
            mockFile.Setup(f => f.FileName).Returns("test.jpg");

            var result = await _imageStorageService.SaveArticleImageAsync(mockFile.Object);

            Assert.That(result, Does.StartWith("/img/ArticleImages"));
        }

        [Test]
        public void SaveImageAsync_ShouldThrowArgumentException_WhenInvalidFileType()
        {
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.Length).Returns(1024);
            mockFile.Setup(f => f.FileName).Returns("test.txt");

            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _imageStorageService.SaveArticleImageAsync(mockFile.Object));
        }
    }
}
