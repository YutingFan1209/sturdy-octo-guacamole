using Microsoft.EntityFrameworkCore;
using MovieShop.Api.Contracts;
using MovieShop.Api.Data;

namespace MovieShop.Api.Repositories;

public class MovieRepository(MovieShopDbContext dbContext) : IMovieRepository
{
    public async Task<MovieDetailsDto?> GetById(
        int id,
        CancellationToken cancellationToken = default)
    {
        var movie = await dbContext.Movies
            .AsNoTracking()
            .AsSplitQuery()
            .Include(movie => movie.MovieGenres)
                .ThenInclude(movieGenre => movieGenre.Genre)
            .Include(movie => movie.MovieCasts)
                .ThenInclude(movieCast => movieCast.CastMember)
            .Include(movie => movie.Trailers)
            .Include(movie => movie.Reviews)
            .SingleOrDefaultAsync(
                movie => movie.Id == id,
                cancellationToken);

        if (movie is null)
        {
            return null;
        }

        var genres = movie.MovieGenres
            .Select(movieGenre => movieGenre.Genre.Name)
            .OrderBy(name => name)
            .ToList();

        var averageRating = movie.Reviews.Count == 0
            ? 0
            : (double)movie.Reviews.Average(review => review.Rating);

        return new MovieDetailsDto(
            movie.Id,
            movie.Title,
            movie.PosterUrl ?? "https://placehold.co/500x750?text=No+Poster",
            movie.BackdropUrl ?? "",
            movie.Overview ?? "",
            movie.Tagline ?? "",
            averageRating,
            genres.FirstOrDefault() ?? "Movie",
            genres,
            movie.ReleaseDate ?? DateTime.MinValue,
            movie.Runtime ?? 0,
            movie.Budget ?? 0,
            movie.Revenue ?? 0,
            movie.Price ?? 0,
            movie.MovieCasts.Select(movieCast => new MovieCastDto(
                movieCast.CastMember.Name ?? "Unknown",
                movieCast.Character,
                movieCast.CastMember.ProfilePath)).ToList(),
            movie.Trailers.Select(trailer => new MovieTrailerDto(
                trailer.Name,
                trailer.TrailerUrl)).ToList());
    }

    public async Task<PagedResultDto<MovieSummaryDto>> GetTop30HighestGrossingAsync(
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        pageNumber = Math.Max(pageNumber, 1);
        pageSize = Math.Clamp(pageSize, 1, 30);

        IQueryable<Models.Movie> movies = dbContext.Movies
            .AsNoTracking()
            .Where(movie => movie.Revenue.HasValue);

        int databaseCount = await movies.CountAsync(cancellationToken);
        int totalCount = Math.Min(databaseCount, 30);
        int skip = (pageNumber - 1) * pageSize;
        int take = Math.Min(pageSize, Math.Max(totalCount - skip, 0));

        IReadOnlyList<MovieSummaryDto> items = take == 0
            ? []
            : await movies
            .OrderByDescending(movie => movie.Revenue)
            .ThenBy(movie => movie.Id)
            .Skip(skip)
            .Take(take)
            .Select(movie => new MovieSummaryDto(
                movie.Id,
                movie.Title,
                movie.ReleaseDate ?? DateTime.MinValue,
                movie.Price ?? 9.90m,
                movie.PosterUrl ?? "https://placehold.co/500x750?text=No+Poster",
                movie.Revenue ?? 0))
            .ToListAsync(cancellationToken);

        return new PagedResultDto<MovieSummaryDto>(
            items,
            pageNumber,
            pageSize,
            totalCount);
    }
}
