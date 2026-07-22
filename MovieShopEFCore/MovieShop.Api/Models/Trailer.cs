namespace MovieShop.Api.Models;

public class Trailer
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string TrailerUrl { get; set; }
    public int MovieId { get; set; }

    public Movie Movie { get; set; } = null!;
}
