namespace MovieShop.Api.Repositories;

public interface IRepository<T> where T : class
{
    Task<T> AddAsync(
        T entity,
        CancellationToken cancellationToken = default);

    Task<T> UpdateAsync(
        T entity,
        CancellationToken cancellationToken = default);
}
