using System.ComponentModel.DataAnnotations;

namespace MovieShopMVC.Models;

public class UserInfo
{
    public int Id { get; set; }
    public string Email { get; set; } = "";
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public DateOnly? DateOfBirth { get; set; }
    public string Token { get; set; } = "";
    public DateTime ExpiresAtUtc { get; set; }
}

public class ProfileViewModel
{
    public required UserInfo User { get; init; }
    public IReadOnlyList<Purchase> Purchases { get; init; } = [];
    public IReadOnlyList<Favorite> Favorites { get; init; } = [];
}

public class RegisterViewModel
{
    [Required, EmailAddress]
    public string Email { get; set; } = "";

    [Required, DataType(DataType.Password), MinLength(6)]
    public string Password { get; set; } = "";

    [Required, Display(Name = "First Name")]
    public string FirstName { get; set; } = "";

    [Required, Display(Name = "Last Name")]
    public string LastName { get; set; } = "";

    [Required, DataType(DataType.Date), Display(Name = "Date Of Birth")]
    public DateTime? DateOfBirth { get; set; }
}

public class LoginViewModel
{
    [Required, EmailAddress]
    public string Email { get; set; } = "";

    [Required, DataType(DataType.Password)]
    public string Password { get; set; } = "";

    public bool RememberMe { get; set; }
}
