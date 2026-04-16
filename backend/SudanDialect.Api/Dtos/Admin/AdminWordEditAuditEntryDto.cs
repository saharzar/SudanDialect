namespace SudanDialect.Api.Dtos.Admin;

public sealed class AdminWordEditAuditEntryDto
{
    public int Id { get; init; }
    public int WordId { get; init; }
    public string WordHeadword { get; init; } = string.Empty;
    public string AdminUserId { get; init; } = string.Empty;
    public string AdminDisplayName { get; init; } = string.Empty;
    public DateTime EditedAt { get; init; }
    public string ActionType { get; init; } = string.Empty;
    public string OldHeadword { get; init; } = string.Empty;
    public string NewHeadword { get; init; } = string.Empty;
    public string OldDefinition { get; init; } = string.Empty;
    public string NewDefinition { get; init; } = string.Empty;
    public bool OldIsActive { get; init; }
    public bool NewIsActive { get; init; }
    public string OldNormalizedHeadword { get; init; } = string.Empty;
    public string NewNormalizedHeadword { get; init; } = string.Empty;
    public string OldNormalizedDefinition { get; init; } = string.Empty;
    public string NewNormalizedDefinition { get; init; } = string.Empty;
    public string? ClientIp { get; init; }
    public string? UserAgent { get; init; }
}
