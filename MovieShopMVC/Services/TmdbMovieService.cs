using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using MovieShopMVC.Models;

namespace MovieShopMVC.Services;

public class TmdbMovieService(HttpClient httpClient, IConfiguration configuration, ILogger<TmdbMovieService> logger)
{
    private const string ImageBase = "https://image.tmdb.org/t/p/";
    private readonly string? _bearerToken = configuration["Tmdb:BearerToken"];
    private readonly string? _apiKey = configuration["Tmdb:ApiKey"];
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public bool IsConfigured => !string.IsNullOrWhiteSpace(_bearerToken) || !string.IsNullOrWhiteSpace(_apiKey);

    public async Task<IReadOnlyList<Movie>> GetPopularMoviesAsync(CancellationToken cancellationToken = default)
    {
        var response = await GetAsync<TmdbListResponse>("movie/popular?language=en-US&page=1", cancellationToken);
        return response?.Results.Select(ToMovie).ToList() ?? [];
    }

    public async Task<Movie?> GetMovieAsync(int id, CancellationToken cancellationToken = default)
    {
        var dto = await GetAsync<TmdbMovie>(
            $"movie/{id}?language=en-US&append_to_response=credits,videos", cancellationToken);
        return dto is null ? null : ToMovie(dto);
    }

    private async Task<T?> GetAsync<T>(string relativeUrl, CancellationToken cancellationToken)
    {
        if (!IsConfigured)
        {
            return default;
        }

        string separator = relativeUrl.Contains('?') ? "&" : "?";
        string url = string.IsNullOrWhiteSpace(_apiKey)
            ? relativeUrl
            : $"{relativeUrl}{separator}api_key={Uri.EscapeDataString(_apiKey)}";

        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        if (!string.IsNullOrWhiteSpace(_bearerToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _bearerToken);
        }

        try
        {
            using HttpResponseMessage response = await httpClient.SendAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning("TMDB returned {StatusCode} for {Url}", response.StatusCode, relativeUrl);
                return default;
            }

            await using Stream stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            return await JsonSerializer.DeserializeAsync<T>(stream, _jsonOptions, cancellationToken);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException)
        {
            logger.LogWarning(ex, "TMDB request failed for {Url}; local movie data will be used", relativeUrl);
            return default;
        }
    }

    private static Movie ToMovie(TmdbMovie dto)
    {
        string genre = dto.Genres?.FirstOrDefault()?.Name
            ?? (dto.GenreIds?.FirstOrDefault() is int id ? GenreNames.GetValueOrDefault(id, "Movie") : "Movie");

        return new Movie
        {
            Id = dto.Id,
            Title = dto.Title,
            Overview = dto.Overview,
            Tagline = dto.Tagline,
            PosterUrl = ImageUrl("w500", dto.PosterPath),
            BackdropUrl = ImageUrl("original", dto.BackdropPath),
            Rating = dto.VoteAverage,
            Genre = genre,
            Genres = dto.Genres?.Select(g => g.Name).ToList() ?? [genre],
            ReleaseDate = DateTime.TryParse(dto.ReleaseDate, out DateTime releaseDate) ? releaseDate : DateTime.MinValue,
            Runtime = dto.Runtime,
            Budget = dto.Budget,
            Revenue = dto.Revenue,
            Price = 9.90M,
            Casts = dto.Credits?.Cast.Take(8).Select(c => new Cast
            {
                Name = c.Name,
                Character = c.Character,
                ProfileUrl = ImageUrl("w185", c.ProfilePath)
            }).ToList() ?? [],
            Trailers = dto.Videos?.Results
                .Where(v => v.Site == "YouTube" && (v.Type == "Trailer" || v.Type == "Teaser"))
                .Take(3)
                .Select(v => new Trailer { Name = v.Name, Url = $"https://www.youtube.com/watch?v={v.Key}" })
                .ToList() ?? []
        };
    }

    private static string ImageUrl(string size, string? path) =>
        string.IsNullOrWhiteSpace(path) ? "https://placehold.co/500x750?text=No+Poster" : $"{ImageBase}{size}{path}";

    private static readonly Dictionary<int, string> GenreNames = new()
    {
        [28] = "Action", [12] = "Adventure", [16] = "Animation", [35] = "Comedy",
        [80] = "Crime", [18] = "Drama", [14] = "Fantasy", [27] = "Horror",
        [10749] = "Romance", [878] = "Science Fiction", [53] = "Thriller"
    };

    private sealed class TmdbListResponse { public List<TmdbMovie> Results { get; set; } = []; }
    private sealed class TmdbMovie
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Overview { get; set; } = "";
        public string Tagline { get; set; } = "";
        [JsonPropertyName("poster_path")] public string? PosterPath { get; set; }
        [JsonPropertyName("backdrop_path")] public string? BackdropPath { get; set; }
        [JsonPropertyName("vote_average")] public double VoteAverage { get; set; }
        [JsonPropertyName("release_date")] public string? ReleaseDate { get; set; }
        [JsonPropertyName("genre_ids")] public List<int>? GenreIds { get; set; }
        public List<TmdbGenre>? Genres { get; set; }
        public int Runtime { get; set; }
        public decimal Budget { get; set; }
        public decimal Revenue { get; set; }
        public TmdbCredits? Credits { get; set; }
        public TmdbVideos? Videos { get; set; }
    }
    private sealed class TmdbGenre { public string Name { get; set; } = ""; }
    private sealed class TmdbCredits { public List<TmdbCast> Cast { get; set; } = []; }
    private sealed class TmdbCast
    {
        public string Name { get; set; } = "";
        public string Character { get; set; } = "";
        [JsonPropertyName("profile_path")] public string? ProfilePath { get; set; }
    }
    private sealed class TmdbVideos { public List<TmdbVideo> Results { get; set; } = []; }
    private sealed class TmdbVideo
    {
        public string Name { get; set; } = "";
        public string Key { get; set; } = "";
        public string Site { get; set; } = "";
        public string Type { get; set; } = "";
    }
}
