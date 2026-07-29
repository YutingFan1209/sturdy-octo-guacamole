using System.ComponentModel.DataAnnotations;

namespace MovieShop.Api.Contracts;

public class RegisterUserRequest
{
    [Required, EmailAddress]
    public string Email { get; set; } = "";

    [Required, MinLength(6)]
    public string Password { get; set; } = "";

    [Required]
    public string FirstName { get; set; } = "";

    [Required]
    public string LastName { get; set; } = "";

    [Required]
    public DateTime? DateOfBirth { get; set; }
}

public class LoginRequest
{
    [Required, EmailAddress]
    public string Email { get; set; } = "";

    [Required]
    public string Password { get; set; } = "";
}

public record UserInfoDto(
    int Id,
    string Email,
    string FirstName,
    string LastName,
    DateOnly? DateOfBirth);
