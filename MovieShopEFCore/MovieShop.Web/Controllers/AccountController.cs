using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieShopMVC.Models;
using MovieShopMVC.Services;

namespace MovieShopMVC.Controllers;

public class AccountController(IAccountService accountService) : Controller
{
    [AllowAnonymous]
    [HttpGet]
    public IActionResult Register() => View(new RegisterViewModel());

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(
        RegisterViewModel model,
        CancellationToken cancellationToken)
    {
        if (model.DateOfBirth.HasValue &&
            model.DateOfBirth.Value > DateTime.Today)
        {
            ModelState.AddModelError(
                nameof(model.DateOfBirth),
                "Date of birth cannot be in the future.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        UserInfo? user = await accountService.RegisterAsync(
            model,
            cancellationToken);

        if (user is null)
        {
            ModelState.AddModelError(
                nameof(model.Email),
                "An account with this email already exists.");
            return View(model);
        }

        await SignInUserAsync(user, false);
        return RedirectToAction("Index", "Movies");
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View(new LoginViewModel());
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(
        LoginViewModel model,
        string? returnUrl = null,
        CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        UserInfo? user = await accountService.ValidateUserAsync(
            model,
            cancellationToken);

        if (user is null)
        {
            ModelState.AddModelError(
                string.Empty,
                "Invalid email or password.");
            return View(model);
        }

        await SignInUserAsync(user, model.RememberMe);

        return LocalRedirect(
            Url.IsLocalUrl(returnUrl)
                ? returnUrl!
                : Url.Action("Index", "Movies")!);
    }

    [Authorize]
    [HttpGet]
    public IActionResult Profile()
    {
        var model = new UserInfo
        {
            Id = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!),
            FirstName = User.FindFirstValue(ClaimTypes.GivenName) ?? "",
            LastName = User.FindFirstValue(ClaimTypes.Surname) ?? "",
            Email = User.FindFirstValue(ClaimTypes.Email) ?? ""
        };

        return View(model);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);

        return RedirectToAction("Index", "Movies");
    }

    private async Task SignInUserAsync(
        UserInfo user,
        bool persistent)
    {
        Claim[] claims =
        [
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.FirstName),
            new(ClaimTypes.GivenName, user.FirstName),
            new(ClaimTypes.Surname, user.LastName),
            new(ClaimTypes.Email, user.Email)
        ];

        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme);

        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = persistent,
                AllowRefresh = true
            });
    }
}
