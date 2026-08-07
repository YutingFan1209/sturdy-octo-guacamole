using MovieShop.Api.Contracts;

namespace MovieShop.Api.Services;

public interface IMovieService
{
    Task<MovieDetailsDto?> GetMovieDetailsAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<PagedResultDto<MovieSummaryDto>> GetTop30HighestGrossingAsync(
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);
}
