namespace MovieShop.Api.Contracts;

public record PurchaseDto(
    int Id,
    string PurchaseNumber,
    decimal TotalPrice,
    DateTime PurchaseDateTime,
    int MovieId,
    string MovieTitle);
