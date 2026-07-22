namespace MovieShop.Api.Models;

public class MovieCrew
{
    public int MovieId { get; set; }
    public int CrewId { get; set; }
    public required string Department { get; set; }
    public required string Job { get; set; }

    public Movie Movie { get; set; } = null!;
    public CrewMember CrewMember { get; set; } = null!;
}
