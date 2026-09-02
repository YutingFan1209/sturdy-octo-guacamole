using System.Security.Claims;
using AssetAccessManager.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace AssetAccessManager.Web.Controllers;

[Authorize(Policy = "AssetAdministrator")]
public sealed class AssetsController(AssetService service) : Controller
{
    public async Task<IActionResult> Index(string? query, int page = 1, CancellationToken ct = default) { ViewBag.Query = query; return View(await service.SearchAsync(query, page, ct)); }
    [HttpPost, ValidateAntiForgeryToken] public async Task<IActionResult> Assign(Guid id, AssignAsset input, CancellationToken ct) { if (!ModelState.IsValid) { TempData["Error"] = "Employee ID is required."; return RedirectToAction(nameof(Index)); } try { var receipt = await service.AssignAsync(id, input, Actor(), ct); TempData["Success"] = $"Assigned to {receipt.EmployeeId}; assignment {receipt.AssignmentId}."; } catch (AssetAlreadyAssignedException e) { TempData["Error"] = e.Message; } return RedirectToAction(nameof(Index)); }
    [HttpPost, ValidateAntiForgeryToken] public async Task<IActionResult> Return(Guid assignmentId, CancellationToken ct) { await service.ReturnAsync(assignmentId, Actor(), ct); TempData["Success"] = "Asset returned."; return RedirectToAction(nameof(Index)); }
    private string Actor() => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException();
}
