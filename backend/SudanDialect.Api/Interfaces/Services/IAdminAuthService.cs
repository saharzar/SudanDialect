using SudanDialect.Api.Dtos.Admin;
using System.Security.Claims;

namespace SudanDialect.Api.Interfaces.Services;

public interface IAdminAuthService
{
    Task<(string AccessToken, DateTime AccessExpiresAtUtc, string RefreshToken, DateTime RefreshExpiresAtUtc, string Username, IReadOnlyCollection<string> Roles)?> LoginAsync(
        string username,
        string password,
        CancellationToken cancellationToken = default);

    Task<AdminLoginResponseDto> BuildSessionResponseAsync(ClaimsPrincipal user, CancellationToken cancellationToken = default);

    Task<(string AccessToken, DateTime AccessExpiresAtUtc, string RefreshToken, DateTime RefreshExpiresAtUtc, string Username, IReadOnlyCollection<string> Roles)?> RefreshAsync(
        string refreshTokenValue,
        CancellationToken cancellationToken = default);

    Task LogoutAsync(ClaimsPrincipal user, string? refreshTokenValue, CancellationToken cancellationToken = default);
}
