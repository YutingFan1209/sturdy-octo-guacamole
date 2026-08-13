using System.Net;
using System.Net.Http.Json;
using MovieShopMVC.Models;

namespace MovieShopMVC.Services;

public class MovieShopApiService(HttpClient httpClient) : IMovieService
{
    public bool IsConfigured => true;

    public async Task<IReadOnlyList<Movie>> GetPopularMoviesAsync(
        CancellationToken cancellationToken = default)
    {
        return await httpClient.GetFromJsonAsync<List<Movie>>(
            "api/movies", cancellationToken) ?? [];
    }

    public async Task<Movie?> GetMovieAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        using HttpResponseMessage response = await httpClient.GetAsync(
            $"api/movies/{id}", cancellationToken);

        return response.StatusCode == HttpStatusCode.NotFound
            ? null
            : await response.EnsureSuccessStatusCode()
                .Content.ReadFromJsonAsync<Movie>(cancellationToken);
    }

    public async Task<PagedResult<Movie>> GetTop30HighestGrossingAsync(
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        return await httpClient.GetFromJsonAsync<PagedResult<Movie>>(
            $"api/movies/top-grossing?pageNumber={pageNumber}&pageSize={pageSize}",
            cancellationToken) ?? new PagedResult<Movie>();
    }

    public async Task SaveReviewAsync(
        Review review,
        CancellationToken cancellationToken = default)
    {
        using HttpResponseMessage response = await httpClient.PostAsJsonAsync(
            $"api/movies/{review.MovieId}/reviews",
            new { review.Rating, review.Comment },
            cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task<Purchase> PurchaseMovieAsync(
        int movieId,
        CancellationToken cancellationToken = default)
    {
        using HttpResponseMessage response = await httpClient.PostAsync(
            $"api/movies/{movieId}/purchase",
            null,
            cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Purchase>(cancellationToken)
            ?? throw new InvalidOperationException("The API returned an empty purchase response.");
    }

    public async Task AddFavoriteAsync(
        int movieId,
        CancellationToken cancellationToken = default)
    {
        using HttpResponseMessage response = await httpClient.PostAsync(
            $"api/movies/{movieId}/favorite",
            null,
            cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}
