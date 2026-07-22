namespace MovieShop.Api.Contracts;

public record MovieSummaryDto(
    int Id,
    string Title,
    DateTime ReleaseDate,
    decimal Price,
    string PosterUrl);
