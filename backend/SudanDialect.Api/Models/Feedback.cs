namespace SudanDialect.Api.Models;

public sealed class Feedback
{
    public int Id { get; set; }
    public int WordId { get; set; }
    public string FeedbackText { get; set; } = string.Empty;
    public bool Resolved { get; set; }
    public DateTime Timestamp { get; set; }

    public Word Word { get; set; } = null!;
}
