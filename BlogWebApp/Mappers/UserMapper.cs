using BlogWebApp.Models;
using BlogWebApp.ViewModels;

namespace BlogWebApp.Mappers
{
    public static class UserMapper
    {
        public static UserViewModel ToViewModel(ApplicationUser user)
        {
            return new UserViewModel
            {
                Id = user.Id,
                UserName = user.UserName!,
                Email = user.Email!,
                CommentsCount = user.Comments?.Count ?? 0,
                LikesCount = user.Likes?.Count ?? 0
            };
        }
    }
}
