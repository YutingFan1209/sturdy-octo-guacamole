using MovieShop.Api.Contracts;

namespace MovieShop.Api.Repositories;

public interface IMovieRepository
{
    Task<MovieDetailsDto?> GetById(int id);
}
