namespace SudanDialect.Api.Dtos.Admin;

public sealed class AdminFeedbackPageDto
{
    public IReadOnlyList<AdminFeedbackItemDto> Items { get; init; } = [];
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
    public int TotalPages { get; init; }
}
