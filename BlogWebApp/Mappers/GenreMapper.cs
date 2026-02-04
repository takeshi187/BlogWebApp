using BlogWebApp.Models;
using BlogWebApp.ViewModels;

namespace BlogWebApp.Mappers
{
    public static class GenreMapper
    {
        public static GenreViewModel ToViewModel(Genre genre)
        {
            return new GenreViewModel
            {
                GenreId = genre.GenreId,
                GenreName = genre.GenreName
            };
        }
    }
}
