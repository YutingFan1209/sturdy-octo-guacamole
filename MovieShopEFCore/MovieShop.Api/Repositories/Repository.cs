using MovieShop.Api.Data;

namespace MovieShop.Api.Repositories;

public class Repository<T>(MovieShopDbContext dbContext)
    : IRepository<T> where T : class
{
    protected MovieShopDbContext DbContext { get; } = dbContext;

    public async Task<T> AddAsync(
        T entity,
        CancellationToken cancellationToken = default)
    {
        DbContext.Set<T>().Add(entity);
        await DbContext.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<T> UpdateAsync(
        T entity,
        CancellationToken cancellationToken = default)
    {
        DbContext.Set<T>().Update(entity);
        await DbContext.SaveChangesAsync(cancellationToken);
        return entity;
    }
}
