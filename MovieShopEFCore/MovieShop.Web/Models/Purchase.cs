using System.ComponentModel.DataAnnotations;

namespace MovieShopMVC.Models;

public class Purchase
{
    public int Id { get; set; }
    public string PurchaseNumber { get; set; } = "";
    public int MovieId { get; set; }
    public string MovieTitle { get; set; } = "";
    public decimal TotalPrice { get; set; }
    public DateTime PurchaseDateTime { get; set; }
}
