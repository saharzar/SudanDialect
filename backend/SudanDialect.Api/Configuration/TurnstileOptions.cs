namespace SudanDialect.Api.Configuration;

public sealed class TurnstileOptions
{
    public const string SectionName = "Turnstile";

    public string SecretKey { get; set; } = string.Empty;
}
