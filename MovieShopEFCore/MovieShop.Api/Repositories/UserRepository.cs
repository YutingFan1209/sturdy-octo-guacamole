using Microsoft.EntityFrameworkCore;
using MovieShop.Api.Data;
using MovieShop.Api.Models;

namespace MovieShop.Api.Repositories;

public class UserRepository(MovieShopDbContext dbContext)
    : Repository<AppUser>(dbContext), IUserRepository
{
    public async Task<AppUser?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        return await DbContext.Users
            .FirstOrDefaultAsync(
                user => user.Email == email,
                cancellationToken);
    }
}
