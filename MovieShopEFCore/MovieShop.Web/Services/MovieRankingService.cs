using MovieShopMVC.Models;

namespace MovieShopMVC.Services;

public class MovieRankingService : IMovieRankingService
{
    public IReadOnlyList<Movie> GetTop30HighestGrossing(
        IEnumerable<Movie> movies)
    {
        ArgumentNullException.ThrowIfNull(movies);

        return movies
            .OrderByDescending(movie => movie.Revenue)
            .Take(30)
            .ToList();
    }
}
