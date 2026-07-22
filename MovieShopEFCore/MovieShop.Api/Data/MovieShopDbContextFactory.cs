using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace MovieShop.Api.Data;

public class MovieShopDbContextFactory : IDesignTimeDbContextFactory<MovieShopDbContext>
{
    public MovieShopDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<MovieShopDbContextFactory>()
            .Build();
        var connectionString = configuration.GetConnectionString("MovieShop")
            ?? throw new InvalidOperationException(
                "The MovieShop connection string is missing from user secrets.");

        var options = new DbContextOptionsBuilder<MovieShopDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        return new MovieShopDbContext(options);
    }
}
