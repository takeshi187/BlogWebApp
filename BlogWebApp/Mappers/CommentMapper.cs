using BlogWebApp.Models;
using BlogWebApp.ViewModels;

namespace BlogWebApp.Mappers
{
    public static class CommentMapper
    {
        public static CommentViewModel ToViewModel(Comment comment)
        {
            return new CommentViewModel
            {
                CommentId = comment.CommentId,
                Content = comment.Content,
                UserName = comment.User?.UserName ?? "Неизвествный",
                CreatedAt = comment.CreatedAt
            };
        }
    }
}
