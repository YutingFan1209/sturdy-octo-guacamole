using System.ComponentModel.DataAnnotations;

namespace MovieShopMVC.Models;

public class Review
{
    public int MovieId { get; set; }
    public int UserId { get; set; }

    [StringLength(60)]
    public string Name { get; set; } = "";

    [Range(1, 10)]
    public decimal Rating { get; set; }

    [Required, StringLength(500)]
    public string Comment { get; set; } = "";

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
