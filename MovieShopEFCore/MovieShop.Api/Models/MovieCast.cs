namespace MovieShop.Api.Models;

public class MovieCast
{
    public int MovieId { get; set; }
    public int CastId { get; set; }
    public required string Character { get; set; }

    public Movie Movie { get; set; } = null!;
    public CastMember CastMember { get; set; } = null!;
}
