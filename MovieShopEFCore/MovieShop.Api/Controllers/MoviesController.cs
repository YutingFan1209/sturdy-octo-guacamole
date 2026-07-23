using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieShop.Api.Contracts;
using MovieShop.Api.Data;
using MovieShop.Api.Models;
using MovieShop.Api.Repositories;

namespace MovieShop.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MoviesController(
    MovieShopDbContext dbContext,
    IMovieRepository movieRepository) : ControllerBase
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
        var movie = await movieRepository.GetById(id);

        if (movie is null)
        {
            return NotFound();
        }

        return Ok(movie);
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
