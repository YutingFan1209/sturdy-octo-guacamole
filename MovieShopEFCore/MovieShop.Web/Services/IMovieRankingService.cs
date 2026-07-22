using MovieShopMVC.Models;

namespace MovieShopMVC.Services;

public interface IMovieRankingService
{
    IReadOnlyList<Movie> GetTop30HighestGrossing(
        IEnumerable<Movie> movies);
}
