using MovieShopMVC.Models;

namespace MovieShopMVC.Services;

public interface IMovieRankingService
{
    Task<PagedResult<Movie>> GetTop30HighestGrossingAsync(
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);
}
