using Microsoft.AspNetCore.Mvc;
using MovieShop.Api.Contracts;
using MovieShop.Api.Services;

namespace MovieShop.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController(
    IAccountService accountService,
    IJwtTokenService jwtTokenService) : ControllerBase
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
    public async Task<ActionResult<LoginResponseDto>> Login(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        UserInfoDto? user = await accountService.ValidateUserAsync(
            request.Email,
            request.Password,
            cancellationToken);

        if (user is null)
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        JwtTokenResult jwt = jwtTokenService.CreateToken(user);

        return Ok(new LoginResponseDto(
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName,
            user.DateOfBirth,
            jwt.Token,
            jwt.ExpiresAtUtc));
    }
}
