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

        public static Genre ToEntity(GenreViewModel genreViewModel)
        {
            return new Genre(genreViewModel.GenreName);
        }
    }
}
