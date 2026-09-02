using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
namespace AssetAccessManager.Web.Controllers;

[Route("development-auth")]
public sealed class DevelopmentAuthController(IWebHostEnvironment environment) : Controller
{
    [HttpGet("sign-in")] public IActionResult SignIn() => environment.IsDevelopment() ? View() : NotFound();
    [HttpPost("sign-in"), ValidateAntiForgeryToken] public async Task<IActionResult> SignIn(string administratorId) { if (!environment.IsDevelopment()) return NotFound(); if (string.IsNullOrWhiteSpace(administratorId)) return View(); var identity = new ClaimsIdentity([new(ClaimTypes.NameIdentifier, administratorId.Trim()), new(ClaimTypes.Name, administratorId.Trim()), new(ClaimTypes.Role, "AssetAdministrator")], CookieAuthenticationDefaults.AuthenticationScheme); await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity)); return RedirectToAction("Index", "Assets"); }
}
