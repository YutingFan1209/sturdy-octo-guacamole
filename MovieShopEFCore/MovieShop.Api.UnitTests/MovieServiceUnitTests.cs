using Moq;
using MovieShop.Api.Contracts;
using MovieShop.Api.Repositories;
using MovieShop.Api.Services;

namespace MovieShop.Api.UnitTests;

[TestClass]
public sealed class MovieServiceUnitTests
{
    private Mock<IMovieRepository> _repositoryMock = null!;
    private MovieService _sut = null!;

    [TestInitialize]
    public void Setup()
    {
        _repositoryMock = new Mock<IMovieRepository>(MockBehavior.Strict);
        _sut = new MovieService(_repositoryMock.Object);
    }

    [TestMethod]
    public async Task GetTop30HighestGrossingAsync_ReturnsRepositoryPage()
    {
        // Arrange
        CancellationToken cancellationToken = new();
        PagedResultDto<MovieSummaryDto> expected = new(
            [
                Movie(1, "Avatar", 2_923_706_026m),
                Movie(2, "Avengers: Endgame", 2_799_439_100m)
            ],
            PageNumber: 2,
            PageSize: 10,
            TotalCount: 30);

        _repositoryMock
            .Setup(repository => repository.GetTop30HighestGrossingAsync(
                2,
                10,
                cancellationToken))
            .ReturnsAsync(expected);

        // Act
        PagedResultDto<MovieSummaryDto> actual =
            await _sut.GetTop30HighestGrossingAsync(2, 10, cancellationToken);

        // Assert
        Assert.AreSame(expected, actual);
        Assert.HasCount(2, actual.Items);
        Assert.AreEqual("Avatar", actual.Items[0].Title);
        Assert.AreEqual(3, actual.TotalPages);
        Assert.IsTrue(actual.HasPreviousPage);
        Assert.IsTrue(actual.HasNextPage);

        _repositoryMock.Verify(
            repository => repository.GetTop30HighestGrossingAsync(
                2,
                10,
                cancellationToken),
            Times.Once);
    }

    [TestMethod]
    public async Task GetTop30HighestGrossingAsync_WhenRepositoryIsEmpty_ReturnsEmptyPage()
    {
        // Arrange
        PagedResultDto<MovieSummaryDto> expected = new([], 1, 10, 0);
        _repositoryMock
            .Setup(repository => repository.GetTop30HighestGrossingAsync(
                1,
                10,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        // Act
        PagedResultDto<MovieSummaryDto> actual =
            await _sut.GetTop30HighestGrossingAsync();

        // Assert
        Assert.IsEmpty(actual.Items);
        Assert.AreEqual(0, actual.TotalPages);
        Assert.IsFalse(actual.HasPreviousPage);
        Assert.IsFalse(actual.HasNextPage);
    }

    [TestMethod]
    public async Task GetTop30HighestGrossingAsync_WhenRepositoryThrows_PropagatesException()
    {
        // Arrange
        _repositoryMock
            .Setup(repository => repository.GetTop30HighestGrossingAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Database unavailable"));

        // Act and assert
        InvalidOperationException exception = await Assert.ThrowsExactlyAsync<InvalidOperationException>(
            () => _sut.GetTop30HighestGrossingAsync());

        Assert.AreEqual("Database unavailable", exception.Message);
    }

    [TestMethod]
    public async Task GetMovieDetailsAsync_ReturnsMovieAndPassesIdToRepository()
    {
        // Arrange
        MovieDetailsDto expected = new(
            7,
            "Inception",
            "poster.jpg",
            "backdrop.jpg",
            "A dream within a dream.",
            "Your mind is the scene of the crime.",
            8.7,
            "Science Fiction",
            ["Science Fiction", "Thriller"],
            new DateTime(2010, 7, 16),
            148,
            160_000_000m,
            839_000_000m,
            9.99m,
            [new MovieCastDto("Leonardo DiCaprio", "Cobb", null)],
            [new MovieTrailerDto("Official Trailer", "https://example.com/trailer")]);

        _repositoryMock
            .Setup(repository => repository.GetById(7, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        // Act
        MovieDetailsDto? actual = await _sut.GetMovieDetailsAsync(7);

        // Assert
        Assert.IsNotNull(actual);
        Assert.AreEqual("Inception", actual.Title);
        Assert.AreEqual(8.7, actual.Rating);
        Assert.HasCount(1, actual.Casts);
        Assert.HasCount(1, actual.Trailers);
        _repositoryMock.Verify(
            repository => repository.GetById(7, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    private static MovieSummaryDto Movie(int id, string title, decimal revenue)
    {
        return new MovieSummaryDto(
            id,
            title,
            new DateTime(2020, 1, 1),
            9.99m,
            "poster.jpg",
            revenue);
    }
}
