using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;

namespace MovieShopMVC.Services;

public class ApiBearerTokenHandler(IHttpContextAccessor httpContextAccessor)
    : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        HttpContext? context = httpContextAccessor.HttpContext;
        string? token = context is null
            ? null
            : await context.GetTokenAsync("access_token");

        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
