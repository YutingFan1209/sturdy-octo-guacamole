using System.ComponentModel.DataAnnotations;

namespace MovieShopMVC.Models;

public class Review
{
    public int MovieId { get; set; }

    [Required, StringLength(60)]
    public string Name { get; set; } = "";

    [Range(1, 10)]
    public int Rating { get; set; }

    [Required, StringLength(500)]
    public string Comment { get; set; } = "";

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
