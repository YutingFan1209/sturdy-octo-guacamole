using System.ComponentModel.DataAnnotations;

namespace MovieShop.Api.Contracts;

public class SaveReviewRequest
{
    [Range(1, 10)]
    public decimal Rating { get; set; }

    [Required, StringLength(500)]
    public string Comment { get; set; } = "";
}

public record ReviewDto(
    int MovieId,
    int UserId,
    decimal Rating,
    string Comment);

public record FavoriteDto(
    int Id,
    int MovieId,
    string MovieTitle,
    string PosterUrl);
