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
}
