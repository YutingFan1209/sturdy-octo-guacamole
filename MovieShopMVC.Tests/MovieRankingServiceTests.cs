using MovieShopMVC.Models;
using MovieShopMVC.Services;

namespace MovieShopMVC.Tests;

public class MovieRankingServiceTests
{
    private readonly MovieRankingService _service = new();

    [Fact]
    public void GetTop30HighestGrossing_SortsByRevenueAndLimitsResultsTo30()
    {
        List<Movie> movies = Enumerable.Range(1, 40)
            .Select(number => new Movie
            {
                Id = number,
                Title = $"Movie {number}",
                Revenue = number * 1_000_000m
            })
            .Reverse()
            .OrderBy(movie => movie.Id % 7)
            .ToList();

        IReadOnlyList<Movie> result = _service.GetTop30HighestGrossing(movies);

        Assert.Equal(30, result.Count);
        Assert.Equal(40_000_000m, result[0].Revenue);
        Assert.Equal(11_000_000m, result[^1].Revenue);
        Assert.True(result.Zip(result.Skip(1),
            (current, next) => current.Revenue >= next.Revenue).All(isDescending => isDescending));
    }

    [Fact]
    public void GetTop30HighestGrossing_WhenFewerThan30Movies_ReturnsAllMoviesInOrder()
    {
        List<Movie> movies =
        [
            new Movie { Id = 1, Title = "Small", Revenue = 10m },
            new Movie { Id = 2, Title = "Big", Revenue = 30m },
            new Movie { Id = 3, Title = "Medium", Revenue = 20m }
        ];

        IReadOnlyList<Movie> result = _service.GetTop30HighestGrossing(movies);

        Assert.Equal(3, result.Count);
        Assert.Equal(["Big", "Medium", "Small"], result.Select(movie => movie.Title));
    }

    [Fact]
    public void GetTop30HighestGrossing_WhenMoviesIsNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            _service.GetTop30HighestGrossing(null!));
    }
}
