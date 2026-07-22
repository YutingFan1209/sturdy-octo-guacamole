namespace MovieShop.Api.Contracts;

public record MovieDetailsDto(
    int Id,
    string Title,
    string PosterUrl,
    string BackdropUrl,
    string Overview,
    string Tagline,
    double Rating,
    string Genre,
    IReadOnlyList<string> Genres,
    DateTime ReleaseDate,
    int Runtime,
    decimal Budget,
    decimal Revenue,
    decimal Price,
    IReadOnlyList<MovieCastDto> Casts,
    IReadOnlyList<MovieTrailerDto> Trailers);

public record MovieCastDto(string Name, string Character, string? ProfileUrl);

public record MovieTrailerDto(string Name, string Url);
