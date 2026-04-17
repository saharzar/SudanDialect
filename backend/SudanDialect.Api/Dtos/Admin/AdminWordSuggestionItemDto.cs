namespace SudanDialect.Api.Dtos.Admin;

public sealed class AdminWordSuggestionItemDto
{
    public int Id { get; init; }
    public string Headword { get; init; } = string.Empty;
    public string Definition { get; init; } = string.Empty;
    public string? Email { get; init; }
    public bool Resolved { get; init; }
    public DateTime Timestamp { get; init; }
}
