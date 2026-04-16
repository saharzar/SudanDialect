namespace SudanDialect.Api.Dtos.Admin;

public sealed class AdminFeedbackItemDto
{
    public int Id { get; init; }
    public int WordId { get; init; }
    public string WordHeadword { get; init; } = string.Empty;
    public string FeedbackText { get; init; } = string.Empty;
    public DateTime Timestamp { get; init; }
    public bool Resolved { get; init; }
}
