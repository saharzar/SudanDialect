using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using SudanDialect.Api.Configuration;
using SudanDialect.Api.Interfaces.Services;

namespace SudanDialect.Api.Services;

public sealed class TurnstileVerificationService : ITurnstileVerificationService
{
    private const string VerifyUrl = "https://challenges.cloudflare.com/turnstile/v0/siteverify";

    private readonly HttpClient _httpClient;
    private readonly TurnstileOptions _options;

    public TurnstileVerificationService(HttpClient httpClient, IOptions<TurnstileOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<bool> VerifyAsync(string token, string? remoteIp, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(_options.SecretKey))
        {
            return false;
        }

        var body = new Dictionary<string, string>
        {
            ["secret"] = _options.SecretKey,
            ["response"] = token
        };

        if (!string.IsNullOrWhiteSpace(remoteIp))
        {
            body["remoteip"] = remoteIp;
        }

        using var response = await _httpClient.PostAsync(VerifyUrl, new FormUrlEncodedContent(body), cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return false;
        }

        var payload = await response.Content.ReadFromJsonAsync<TurnstileVerifyResponse>(cancellationToken: cancellationToken);
        return payload?.Success == true;
    }

    private sealed class TurnstileVerifyResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; init; }
    }
}
