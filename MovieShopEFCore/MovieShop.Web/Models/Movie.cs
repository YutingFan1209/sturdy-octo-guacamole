namespace MovieShopMVC.Models;

public class Movie
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string PosterUrl { get; set; } = "";
    public string BackdropUrl { get; set; } = "";
    public string Overview { get; set; } = "";
    public string Tagline { get; set; } = "";
    public double Rating { get; set; }
    public string Genre { get; set; } = "";
    public List<string> Genres { get; set; } = [];
    public DateTime ReleaseDate { get; set; }
    public int Runtime { get; set; }
    public decimal Budget { get; set; }
    public decimal Revenue { get; set; }
    public decimal Price { get; set; }
    public List<Cast> Casts { get; set; } = [];
    public List<Trailer> Trailers { get; set; } = [];
    public List<Review> Reviews { get; set; } = [];
}
