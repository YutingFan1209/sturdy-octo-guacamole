using MovieShop.Api.Models;

namespace MovieShop.Api.Repositories;

public interface IUserRepository : IRepository<AppUser>
{
    Task<AppUser?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default);
}
