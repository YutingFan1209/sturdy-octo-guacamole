using System.ComponentModel.DataAnnotations;

namespace MovieShopMVC.Models;

public class AppUser
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Email { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public DateTime DateOfBirth { get; set; }
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
