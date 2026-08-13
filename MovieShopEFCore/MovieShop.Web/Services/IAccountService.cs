using MovieShopMVC.Models;

namespace MovieShopMVC.Services;

public interface IAccountService
{
    Task<UserInfo?> RegisterAsync(
        RegisterViewModel model,
        CancellationToken cancellationToken = default);

    Task<UserInfo?> ValidateUserAsync(
        LoginViewModel model,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Purchase>> GetPurchasesAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Favorite>> GetFavoritesAsync(
        CancellationToken cancellationToken = default);
}
