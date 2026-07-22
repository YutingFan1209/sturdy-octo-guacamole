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
}
