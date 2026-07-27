using MovieShopMVC.Models;

namespace MovieShopMVC.Services;

public class MovieRankingService(IMovieService movieService) : IMovieRankingService
{
    public async Task<PagedResult<Movie>> GetTop30HighestGrossingAsync(
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        return await movieService.GetTop30HighestGrossingAsync(
            pageNumber,
            pageSize,
            cancellationToken);
    }
}
