using MovieShop.Api.Contracts;

namespace MovieShop.Api.Services;

public interface IJwtTokenService
{
    JwtTokenResult CreateToken(UserInfoDto user);
}

public record JwtTokenResult(string Token, DateTime ExpiresAtUtc);
