namespace SudanDialect.Api.Models;

public sealed class Audit
{
    public int Id { get; set; }
    public int WordId { get; set; }
    public string AdminUserId { get; set; } = string.Empty;
    public DateTime EditedAt { get; set; }
    public string ActionType { get; set; } = string.Empty;
    public string OldHeadword { get; set; } = string.Empty;
    public string NewHeadword { get; set; } = string.Empty;
    public string OldDefinition { get; set; } = string.Empty;
    public string NewDefinition { get; set; } = string.Empty;
    public bool OldIsActive { get; set; }
    public bool NewIsActive { get; set; }
    public string OldNormalizedHeadword { get; set; } = string.Empty;
    public string NewNormalizedHeadword { get; set; } = string.Empty;
    public string OldNormalizedDefinition { get; set; } = string.Empty;
    public string NewNormalizedDefinition { get; set; } = string.Empty;
    public string? ClientIp { get; set; }
    public string? UserAgent { get; set; }

    public Word Word { get; set; } = null!;
}
