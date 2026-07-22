using System.ComponentModel.DataAnnotations;

namespace MovieShopMVC.Models;

public class Purchase
{
    public Guid ConfirmationNumber { get; set; } = Guid.NewGuid();
    public int MovieId { get; set; }
    public string MovieTitle { get; set; } = "";
    public decimal Price { get; set; }

    [Required, EmailAddress]
    public string Email { get; set; } = "";

    public DateTime PurchasedAt { get; set; } = DateTime.Now;
}
