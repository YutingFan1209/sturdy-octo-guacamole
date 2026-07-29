using Microsoft.AspNetCore.Mvc;
using MovieShop.Api.Contracts;
using MovieShop.Api.Services;

namespace MovieShop.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController(IAccountService accountService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<UserInfoDto>> Register(
        RegisterUserRequest request,
        CancellationToken cancellationToken)
    {
        UserInfoDto? user = await accountService.RegisterAsync(
            request,
            cancellationToken);

        if (user is null)
        {
            return Conflict(new { message = "An account with this email already exists." });
        }

        return Ok(user);
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserInfoDto>> Login(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        UserInfoDto? user = await accountService.ValidateUserAsync(
            request.Email,
            request.Password,
            cancellationToken);

        return user is null
            ? Unauthorized(new { message = "Invalid email or password." })
            : Ok(user);
    }
}
