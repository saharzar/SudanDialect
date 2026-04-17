namespace SudanDialect.Api.Models;

public sealed class WordSuggestion
{
    public int Id { get; set; }
    public string Headword { get; set; } = string.Empty;
    public string Definition { get; set; } = string.Empty;
    public string? Email { get; set; }
    public bool Resolved { get; set; }
    public DateTime Timestamp { get; set; }
}
