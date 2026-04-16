namespace SudanDialect.Api.Interfaces.Services;

public interface ITurnstileVerificationService
{
    Task<bool> VerifyAsync(string token, string? remoteIp, CancellationToken cancellationToken = default);
}
