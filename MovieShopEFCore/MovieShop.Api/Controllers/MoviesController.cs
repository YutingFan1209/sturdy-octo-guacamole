using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieShop.Api.Contracts;
using MovieShop.Api.Data;
using MovieShop.Api.Models;

namespace MovieShop.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MoviesController(MovieShopDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<MovieSummaryDto>>> GetMovies()
    {
        var movies = await dbContext.Movies
            .AsNoTracking()
            .OrderBy(movie => movie.Title)
            .Take(60)
            .Select(movie => new MovieSummaryDto(
                movie.Id,
                movie.Title,
                movie.ReleaseDate ?? DateTime.MinValue,
                movie.Price ?? 9.90m,
                movie.PosterUrl ?? "https://placehold.co/500x750?text=No+Poster"))
            .ToListAsync();

        return Ok(movies);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<MovieDetailsDto>> GetMovie(int id)
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
            .Where(movie => movie.Id == id)
            .SingleOrDefaultAsync();

        if (movie is null)
        {
            return NotFound();
        }

        var genres = movie.MovieGenres
            .Select(movieGenre => movieGenre.Genre.Name)
            .OrderBy(name => name)
            .ToList();
        var response = new MovieDetailsDto(
            movie.Id,
            movie.Title,
            movie.PosterUrl ?? "https://placehold.co/500x750?text=No+Poster",
            movie.BackdropUrl ?? "",
            movie.Overview ?? "",
            movie.Tagline ?? "",
            movie.Reviews.Count == 0
                ? 0
                : (double)movie.Reviews.Average(review => review.Rating),
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

        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<MovieSummaryDto>> CreateMovie(
        CreateMovieRequest request)
    {
        var movie = new Movie
        {
            Title = request.Title,
            Overview = request.Overview,
            ReleaseDate = request.ReleaseDate,
            Price = request.Price,
            PosterUrl = request.PosterUrl,
            CreatedDate = DateTime.UtcNow
        };

        dbContext.Movies.Add(movie);
        await dbContext.SaveChangesAsync();

        var response = new MovieSummaryDto(
            movie.Id,
            movie.Title,
            movie.ReleaseDate ?? DateTime.MinValue,
            movie.Price ?? 9.90m,
            movie.PosterUrl ?? "https://placehold.co/500x750?text=No+Poster");

        return CreatedAtAction(nameof(GetMovie), new { id = movie.Id }, response);
    }
}
