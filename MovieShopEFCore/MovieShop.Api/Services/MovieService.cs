using MovieShop.Api.Contracts;
using MovieShop.Api.Repositories;

namespace MovieShop.Api.Services;

public sealed class MovieService(IMovieRepository movieRepository) : IMovieService
{
    public Task<MovieDetailsDto?> GetMovieDetailsAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return movieRepository.GetById(id, cancellationToken);
    }

    public Task<PagedResultDto<MovieSummaryDto>> GetTop30HighestGrossingAsync(
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        return movieRepository.GetTop30HighestGrossingAsync(
            pageNumber,
            pageSize,
            cancellationToken);
    }
}
