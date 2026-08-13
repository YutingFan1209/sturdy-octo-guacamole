using MovieShopMVC.Models;
using MovieShopMVC.Services;

namespace MovieShopMVC.Tests;

public class MovieRankingServiceTests
{
    [Fact]
    public async Task GetTop30HighestGrossingAsync_DelegatesPaginationToMovieService()
    {
        var expected = new PagedResult<Movie>
        {
            Items = [new Movie { Id = 7, Title = "Avatar", Revenue = 2_900_000_000m }],
            PageNumber = 2,
            PageSize = 5,
            TotalCount = 30
        };
        var movieService = new StubMovieService(expected);
        var service = new MovieRankingService(movieService);

        PagedResult<Movie> result = await service.GetTop30HighestGrossingAsync(2, 5);

        Assert.Same(expected, result);
        Assert.Equal(2, movieService.PageNumber);
        Assert.Equal(5, movieService.PageSize);
    }

    private sealed class StubMovieService(PagedResult<Movie> result) : IMovieService
    {
        public int PageNumber { get; private set; }
        public int PageSize { get; private set; }
        public bool IsConfigured => true;

        public Task<PagedResult<Movie>> GetTop30HighestGrossingAsync(
            int pageNumber = 1,
            int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            return Task.FromResult(result);
        }

        public Task<IReadOnlyList<Movie>> GetPopularMoviesAsync(
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<Movie>>([]);

        public Task<Movie?> GetMovieAsync(
            int id,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<Movie?>(null);

        public Task SaveReviewAsync(
            Review review,
            CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<Purchase> PurchaseMovieAsync(
            int movieId,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(new Purchase());

        public Task AddFavoriteAsync(
            int movieId,
            CancellationToken cancellationToken = default) => Task.CompletedTask;
    }
}
