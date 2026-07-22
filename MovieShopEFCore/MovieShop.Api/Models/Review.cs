namespace MovieShop.Api.Models;

public class Review
{
    public int MovieId { get; set; }
    public int UserId { get; set; }
    public decimal Rating { get; set; }
    public string? ReviewText { get; set; }

    public Movie Movie { get; set; } = null!;
    public AppUser User { get; set; } = null!;
}
