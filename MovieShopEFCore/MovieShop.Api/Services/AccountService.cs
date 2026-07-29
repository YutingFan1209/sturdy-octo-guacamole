using System.Security.Cryptography;
using MovieShop.Api.Contracts;
using MovieShop.Api.Models;
using MovieShop.Api.Repositories;

namespace MovieShop.Api.Services;

public class AccountService(IUserRepository userRepository) : IAccountService
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 100_000;

    public async Task<UserInfoDto?> RegisterAsync(
        RegisterUserRequest request,
        CancellationToken cancellationToken = default)
    {
        string email = request.Email.Trim().ToLowerInvariant();

        AppUser? existingUser = await userRepository.GetByEmailAsync(
            email,
            cancellationToken);

        if (existingUser is not null)
        {
            return null;
        }

        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
        byte[] hash = HashPassword(request.Password, salt);

        var user = new AppUser
        {
            Email = email,
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            DateOfBirth = request.DateOfBirth.HasValue
                ? DateOnly.FromDateTime(request.DateOfBirth.Value)
                : null,
            Salt = Convert.ToBase64String(salt),
            HashedPassword = Convert.ToBase64String(hash),
            AccessFailedCount = 0,
            IsLocked = false
        };

        await userRepository.AddAsync(user, cancellationToken);

        return ToUserInfo(user);
    }

    public async Task<UserInfoDto?> ValidateUserAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        string normalizedEmail = email.Trim().ToLowerInvariant();

        AppUser? user = await userRepository.GetByEmailAsync(
            normalizedEmail,
            cancellationToken);

        if (user is null ||
            user.IsLocked == true ||
            string.IsNullOrWhiteSpace(user.Salt) ||
            string.IsNullOrWhiteSpace(user.HashedPassword))
        {
            return null;
        }

        byte[] salt;
        byte[] expectedHash;
        try
        {
            salt = Convert.FromBase64String(user.Salt);
            expectedHash = Convert.FromBase64String(user.HashedPassword);
        }
        catch (FormatException)
        {
            return null;
        }

        byte[] actualHash = HashPassword(password, salt);
        bool passwordMatches =
            CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);

        if (!passwordMatches)
        {
            user.AccessFailedCount = (user.AccessFailedCount ?? 0) + 1;
            await userRepository.UpdateAsync(user, cancellationToken);
            return null;
        }

        user.AccessFailedCount = 0;
        user.LastLoginDateTime = DateTime.UtcNow;
        await userRepository.UpdateAsync(user, cancellationToken);

        return ToUserInfo(user);
    }

    private static byte[] HashPassword(string password, byte[] salt) =>
        Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            Iterations,
            HashAlgorithmName.SHA256,
            HashSize);

    private static UserInfoDto ToUserInfo(AppUser user) =>
        new(
            user.Id,
            user.Email ?? "",
            user.FirstName ?? "",
            user.LastName ?? "",
            user.DateOfBirth);
}
