using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieShop.Api.Contracts;
using MovieShop.Api.Data;
using MovieShop.Api.Models;
using MovieShop.Api.Repositories;
using MovieShop.Api.Services;

namespace MovieShop.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MoviesController(
    MovieShopDbContext dbContext,
    IMovieService movieService) : ControllerBase
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
                movie.PosterUrl ?? "https://placehold.co/500x750?text=No+Poster",
                movie.Revenue ?? 0))
            .ToListAsync();

        return Ok(movies);
    }

    [HttpGet("upcoming")]
    public async Task<ActionResult<IReadOnlyList<MovieSummaryDto>>> GetUpcomingMovies(
        CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow.Date;
        var movies = await dbContext.Movies
            .AsNoTracking()
            .Where(movie => movie.ReleaseDate >= today)
            .OrderBy(movie => movie.ReleaseDate)
            .ThenBy(movie => movie.Title)
            .Take(30)
            .Select(movie => new MovieSummaryDto(
                movie.Id,
                movie.Title,
                movie.ReleaseDate ?? DateTime.MinValue,
                movie.Price ?? 9.90m,
                movie.PosterUrl ?? "https://placehold.co/500x750?text=No+Poster",
                movie.Revenue ?? 0))
            .ToListAsync(cancellationToken);

        if (movies.Count == 0)
        {
            return NotFound(new { message = "No upcoming movies found." });
        }

        return Ok(movies);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<MovieDetailsDto>> GetMovie(
        int id,
        CancellationToken cancellationToken)
    {
        var movie = await movieService.GetMovieDetailsAsync(id, cancellationToken);

        if (movie is null)
        {
            return NotFound();
        }

        return Ok(movie);
    }

    [HttpGet("top-grossing")]
    public async Task<ActionResult<PagedResultDto<MovieSummaryDto>>> GetTop30HighestGrossing(
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var movies = await movieService.GetTop30HighestGrossingAsync(
            pageNumber,
            pageSize,
            cancellationToken);

        return Ok(movies);
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
            movie.PosterUrl ?? "https://placehold.co/500x750?text=No+Poster",
            movie.Revenue ?? 0);

        return CreatedAtAction(nameof(GetMovie), new { id = movie.Id }, response);
    }
}
