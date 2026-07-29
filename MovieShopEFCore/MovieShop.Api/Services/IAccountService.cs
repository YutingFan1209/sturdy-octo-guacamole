using MovieShop.Api.Contracts;

namespace MovieShop.Api.Services;

public interface IAccountService
{
    Task<UserInfoDto?> RegisterAsync(
        RegisterUserRequest request,
        CancellationToken cancellationToken = default);

    Task<UserInfoDto?> ValidateUserAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default);
}
