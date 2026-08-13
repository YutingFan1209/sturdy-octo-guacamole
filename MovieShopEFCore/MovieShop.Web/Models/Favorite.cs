namespace MovieShopMVC.Models;

public class Favorite
{
    public int Id { get; set; }
    public int MovieId { get; set; }
    public string MovieTitle { get; set; } = "";
    public string PosterUrl { get; set; } = "";
}
