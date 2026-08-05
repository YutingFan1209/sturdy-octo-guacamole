using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieShop.Api.Contracts;
using MovieShop.Api.Data;

namespace MovieShop.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class UsersController(MovieShopDbContext dbContext) : ControllerBase
{
    [HttpGet("purchases")]
    public async Task<ActionResult<IReadOnlyList<PurchaseDto>>> GetPurchases(
        CancellationToken cancellationToken)
    {
        string? userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized(new { message = "The token does not contain a valid user ID." });
        }

        var purchases = await dbContext.Purchases
            .AsNoTracking()
            .Where(purchase => purchase.UserId == userId)
            .OrderByDescending(purchase => purchase.PurchaseDateTime)
            .Select(purchase => new PurchaseDto(
                purchase.Id,
                purchase.PurchaseNumber,
                purchase.TotalPrice,
                purchase.PurchaseDateTime,
                purchase.MovieId,
                purchase.Movie.Title))
            .ToListAsync(cancellationToken);

        return Ok(purchases);
    }
}
