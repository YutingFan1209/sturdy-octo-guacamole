namespace MovieShop.Api.Models;

public class Purchase
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public required string PurchaseNumber { get; set; }
    public decimal TotalPrice { get; set; }
    public DateTime PurchaseDateTime { get; set; }
    public int MovieId { get; set; }

    public AppUser User { get; set; } = null!;
    public Movie Movie { get; set; } = null!;
}
