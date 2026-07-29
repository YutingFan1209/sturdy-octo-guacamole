using System.Net;
using System.Net.Http.Json;
using MovieShopMVC.Models;

namespace MovieShopMVC.Services;

public class AccountApiService(HttpClient httpClient) : IAccountService
{
    public async Task<UserInfo?> RegisterAsync(
        RegisterViewModel model,
        CancellationToken cancellationToken = default)
    {
        using HttpResponseMessage response = await httpClient.PostAsJsonAsync(
            "api/accounts/register",
            model,
            cancellationToken);

        return response.StatusCode == HttpStatusCode.Conflict
            ? null
            : await response.EnsureSuccessStatusCode()
                .Content.ReadFromJsonAsync<UserInfo>(
                    cancellationToken);
    }

    public async Task<UserInfo?> ValidateUserAsync(
        LoginViewModel model,
        CancellationToken cancellationToken = default)
    {
        using HttpResponseMessage response = await httpClient.PostAsJsonAsync(
            "api/accounts/login",
            model,
            cancellationToken);

        return response.StatusCode == HttpStatusCode.Unauthorized
            ? null
            : await response.EnsureSuccessStatusCode()
                .Content.ReadFromJsonAsync<UserInfo>(
                    cancellationToken);
    }
}
