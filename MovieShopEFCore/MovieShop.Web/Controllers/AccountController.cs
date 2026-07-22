using System.Collections.Concurrent;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MovieShopMVC.Models;

namespace MovieShopMVC.Controllers;

public class AccountController : Controller
{
    private static readonly ConcurrentDictionary<string, AppUser> Users = new(StringComparer.OrdinalIgnoreCase);
    private readonly PasswordHasher<AppUser> _passwordHasher = new();

    [HttpGet]
    public IActionResult Register() => View(new RegisterViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (model.DateOfBirth.HasValue && model.DateOfBirth.Value > DateTime.Today)
        {
            ModelState.AddModelError(nameof(model.DateOfBirth), "Date of birth cannot be in the future.");
        }
        if (Users.ContainsKey(model.Email))
        {
            ModelState.AddModelError(nameof(model.Email), "An account with this email already exists.");
        }
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = new AppUser
        {
            Email = model.Email.Trim(),
            FirstName = model.FirstName.Trim(),
            LastName = model.LastName.Trim(),
            DateOfBirth = model.DateOfBirth!.Value
        };
        user.PasswordHash = _passwordHasher.HashPassword(user, model.Password);

        if (!Users.TryAdd(user.Email, user))
        {
            ModelState.AddModelError(nameof(model.Email), "An account with this email already exists.");
            return View(model);
        }

        await SignInAsync(user, false);
        return RedirectToAction("Index", "Movies");
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View(new LoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (!Users.TryGetValue(model.Email.Trim(), out AppUser? user) ||
            _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password) == PasswordVerificationResult.Failed)
        {
            ModelState.AddModelError(string.Empty, "Invalid email or password.");
            return View(model);
        }

        await SignInAsync(user, model.RememberMe);
        return LocalRedirect(Url.IsLocalUrl(returnUrl) ? returnUrl! : Url.Action("Index", "Movies")!);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Movies");
    }

    private async Task SignInAsync(AppUser user, bool persistent)
    {
        Claim[] claims =
        [
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.FirstName),
            new(ClaimTypes.Email, user.Email)
        ];
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
            new AuthenticationProperties { IsPersistent = persistent });
    }
}
