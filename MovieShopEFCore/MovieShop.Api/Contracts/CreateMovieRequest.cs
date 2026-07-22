using System.ComponentModel.DataAnnotations;

namespace MovieShop.Api.Contracts;

public class CreateMovieRequest
{
    [Required]
    [MaxLength(256)]
    public required string Title { get; set; }

    public string? Overview { get; set; }
    public DateTime? ReleaseDate { get; set; }

    [Range(0, 999.99)]
    public decimal? Price { get; set; }

    public string? PosterUrl { get; set; }
}
