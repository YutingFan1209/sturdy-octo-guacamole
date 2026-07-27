using MovieShopMVC.Models;

namespace MovieShopMVC.Services;

public interface IMovieService
{
    bool IsConfigured { get; }

    Task<IReadOnlyList<Movie>> GetPopularMoviesAsync(
        CancellationToken cancellationToken = default);

    Task<Movie?> GetMovieAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<PagedResult<Movie>> GetTop30HighestGrossingAsync(
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);
}
